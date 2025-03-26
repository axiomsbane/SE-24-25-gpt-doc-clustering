// using System.Collections.Generic;
// using System.Threading.Tasks;
// using GPTDocumentClustering.Models;
// using GPTDocumentClustering.Services.Embedding;
// using Moq;
// using NUnit.Framework;
//
// namespace GPTDocumentClustering.Tests
// {
//     [TestFixture]
//     public class EmbeddingServiceTests
//     {
//         [Test]
//         public async Task GenerateEmbeddings_ShouldGenerateEmbeddingsForDocuments()
//         {
//             // Arrange
//             var documents = new List<Document>
//             {
//                 new Document { SerialNo = 1, Content = "Test1" },
//                 new Document { SerialNo = 2, Content = "Test2" }
//             };
//
//             var mockGenerator = new Mock<EmbeddingGenerator>();
//             mockGenerator.Setup(g => g.GenerateEmbeddings(It.IsAny<string>()))
//                          .ReturnsAsync(new double[] { 0.5, 0.5 });
//
//             var embeddingService = new EmbeddingService();
//
//             // Act
//             var result = await embeddingService.GenerateEmbeddings(documents);
//
//             // Assert
//             Assert.That(result.All(d => d.Embedding != null), Is.True);
//             Assert.That(result.All(d => d.Embedding.Length == 2), Is.True);
//         }
//     }
// }
