using NUnit.Framework;
using Moq;
using GPTDocumentClustering.Interfaces.InputData;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.InputData;
using OpenAI.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GPTDocumentClustering.Tests
{
    public class ProgramTests
    {
        private Mock<IReadInputData> _mockService;
        private Mock<EmbeddingClient> _mockEmbeddingClient;

        [SetUp]
        public void Setup()
        {
            // Mock the IReadInputData service
            _mockService = new Mock<IReadInputData>();

            // Mock the EmbeddingClient
            _mockEmbeddingClient = new Mock<EmbeddingClient>("text-embedding-3-small", "fake-api-key");
        }

        [Test]
        public void TestDocumentReading()
        {
            // Arrange: Prepare a list of documents to return from the mock service
            var documents = new List<Document>
            {
                new Document { Content = "This is document 1.", Category = "Category1" },
                new Document { Content = "This is document 2.", Category = "Category2" },
                new Document { Content = "This is document 3.", Category = "Category3" }
            };

            _mockService.Setup(service => service.ReadDocuments()).Returns(documents);

            // Act: Call the method that uses IReadInputData
            var result = _mockService.Object.ReadDocuments();

            // Assert: Verify the documents returned match the expected output
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("This is document 1.", result[0].Content);
            Assert.AreEqual("Category1", result[0].Category);
        }

        [Test]
        public void TestDocumentContentCleaning()
        {
            // Arrange
            var document = new Document { Content = "This is document 1.\r\n", Category = "Category1" };

            // Act: Clean the document content using Regex.Replace
            var cleanedContent = Regex.Replace(document.Content.Trim(), @"\r\n?|\n", " ");

            // Assert: Verify the content is cleaned as expected
            Assert.AreEqual("This is document 1.", cleanedContent);
        }

        [Test]
        public void TestEmbeddingGeneration()
        {
            // Arrange
            var embeddingMock = new Mock<OpenAIEmbedding>();
            var documents = new List<Document>
            {
                new Document { Content = "This is document 1.", Category = "Category1" },
                new Document { Content = "This is document 2.", Category = "Category2" },
                new Document { Content = "This is document 3.", Category = "Category3" }
            };

            var embeddingResult = new float[] { 0.1f, 0.2f, 0.3f };
            embeddingMock.Setup(embedding => embedding.ToFloats()).Returns(new ReadOnlyMemory<float>(embeddingResult));

            _mockEmbeddingClient.Setup(client => client.GenerateEmbedding(It.IsAny<string>(), It.IsAny<EmbeddingGenerationOptions>()))
                .Returns(embeddingMock.Object);

            // Act: Generate embeddings for the first document
            var embedding = _mockEmbeddingClient.Object.GenerateEmbedding(documents[0].Content, new EmbeddingGenerationOptions { Dimensions = 15 });
            var vector = embedding.ToFloats();

            // Assert: Verify the embeddings are generated correctly
            Assert.AreEqual(3, vector.Length);
            Assert.AreEqual(0.1f, vector[0]);
            Assert.AreEqual(0.2f, vector[1]);
            Assert.AreEqual(0.3f, vector[2]);
        }

        [Test]
        public void TestEmbeddingOutputFormatting()
        {
            // Arrange
            var embeddingMock = new Mock<OpenAIEmbedding>();
            var documents = new List<Document>
            {
                new Document { Content = "This is document 1.", Category = "Category1" }
            };

            var embeddingResult = new float[] { 0.123456f, 0.654321f, 0.987654f };
            embeddingMock.Setup(embedding => embedding.ToFloats()).Returns(new ReadOnlyMemory<float>(embeddingResult));

            _mockEmbeddingClient.Setup(client => client.GenerateEmbedding(It.IsAny<string>(), It.IsAny<EmbeddingGenerationOptions>()))
                .Returns(embeddingMock.Object);

            // Act: Generate embedding for the first document
            var embedding = _mockEmbeddingClient.Object.GenerateEmbedding(documents[0].Content, new EmbeddingGenerationOptions { Dimensions = 15 });
            var vector = embedding.ToFloats();

            // Assert: Verify that the output is formatted to 4 decimal places
            var formattedVector = string.Join(", ", vector.ToArray().Select(x => x.ToString("F4")));
            Assert.AreEqual("0.1235, 0.6543, 0.9877", formattedVector);
        }
    }
}
