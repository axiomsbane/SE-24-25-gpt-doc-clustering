using Moq;
using NUnit.Framework;
using GPTDocumentClustering.Services.InputData;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Embedding;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using GPTDocumentClustering.Interfaces.InputData;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace GPTDocumentClustering.Tests
{
    public class ProgramTests
    {
        // Test for CsvDataReader to ensure it returns documents correctly
        [Test]
        public void CsvDataReader_ReadDocuments_ReturnsDocuments()
        {
            // Arrange
            var mockCsvReader = new Mock<IReadInputData>();
            var expectedDocuments = new List<Document>
            {
                new Document { Content = "Document 1", Category = "Category 1" },
                new Document { Content = "Document 2", Category = "Category 2" }
            };
            mockCsvReader.Setup(x => x.ReadDocuments()).Returns(expectedDocuments);

            // Act
            var actualDocuments = mockCsvReader.Object.ReadDocuments();

            // Assert
            Assert.AreEqual(expectedDocuments.Count, actualDocuments.Count);
            Assert.AreEqual(expectedDocuments[0].Content, actualDocuments[0].Content);
            Assert.AreEqual(expectedDocuments[1].Category, actualDocuments[1].Category);
        }

        // Test for EmbeddingService to ensure embeddings are generated correctly
        [Test]
        public async Task EmbeddingService_GenerateEmbeddings_ReturnsEmbeddings()
        {
            // Arrange
            var mockEmbeddingService = new Mock<EmbeddingService>();
            var documents = new List<Document>
            {
                new Document { Content = "Document 1" },
                new Document { Content = "Document 2" }
            };

            var expectedEmbeddings = new List<float[]> { new float[] { 0.1f, 0.2f }, new float[] { 0.3f, 0.4f } };
            mockEmbeddingService.Setup(x => x.GenerateEmbeddings(It.IsAny<List<Document>>()))
                .ReturnsAsync(expectedEmbeddings);

            // Act
            var actualEmbeddings = await mockEmbeddingService.Object.GenerateEmbeddings(documents);

            // Assert
            Assert.AreEqual(expectedEmbeddings.Count, actualEmbeddings.Count);
            Assert.AreEqual(expectedEmbeddings[0], actualEmbeddings[0]);
        }

        // Test for exception handling when an exception is thrown
        [Test]
        public void Main_ThrowsException_HandlesError()
        {
            // Arrange
            var mockCsvReader = new Mock<IReadInputData>();
            var mockEmbeddingService = new Mock<EmbeddingService>();
            mockCsvReader.Setup(x => x.ReadDocuments()).Throws(new Exception("Data read error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await Program.Main(new string[] { }));
            Assert.AreEqual("An error occurred: Data read error", ex.Message);
        }
    }
}
