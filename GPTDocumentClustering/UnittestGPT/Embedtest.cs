using System.Threading.Tasks;
using GPTDocumentClustering.Services.Embedding;
using Moq;
using NUnit.Framework;

namespace GPTDocumentClustering.Tests
{
    [TestFixture]
    public class EmbeddingGeneratorTests
    {
        [Test]
        public async Task GenerateEmbeddings_ShouldReturnValidEmbedding()
        {
            // Arrange
            var mockGenerator = new Mock<EmbeddingGenerator>();
            mockGenerator.Setup(g => g.GenerateEmbeddings(It.IsAny<string>()))
                         .ReturnsAsync(new double[] { 0.1, 0.2, 0.3 });

            var generator = new EmbeddingGenerator();

            // Act
            double[] result = await generator.GenerateEmbeddings("test");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(3));
        }
    }
}
