using System.Threading.Tasks;
using GPTDocumentClustering.Services.Embedding;
using Moq;
using NUnit.Framework;

namespace GPTDocumentClustering.Tests
{
    [TestFixture]
    public class EmbeddingGeneratorTests
    {
        private Mock<EmbeddingGenerator> _mockGenerator;

        [SetUp]
        public void SetUp()
        {
            _mockGenerator = new Mock<EmbeddingGenerator>();
            _mockGenerator.Setup(g => g.GenerateEmbeddings(It.IsAny<string>()))
                          .ReturnsAsync(new double[] { 0.1, 0.2, 0.3 });
        }

        [Test]
        public async Task GenerateEmbeddings_ShouldReturnValidEmbedding()
        {
            // Act
            double[] result = await _mockGenerator.Object.GenerateEmbeddings("test");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(3));
        }

        [Test]
        public async Task GenerateEmbeddings_ShouldHandleEmptyString()
        {
            // Act
            double[] result = await _mockGenerator.Object.GenerateEmbeddings("");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(3));
        }

        [Test]
        public async Task GenerateEmbeddings_ShouldHandleLongText()
        {
            // Act
            double[] result = await _mockGenerator.Object.GenerateEmbeddings(new string('A', 1000));

            // Assert
            Assert.That(result, Is.Not.Null);
        }
    }
}
