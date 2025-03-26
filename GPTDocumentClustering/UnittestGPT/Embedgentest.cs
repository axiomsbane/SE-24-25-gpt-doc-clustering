// //using System.Threading.Tasks;
// //using GPTDocumentClustering.Services.Embedding;
// //using Moq;
// //using NUnit.Framework;
//
// //namespace GPTDocumentClustering.Tests
// //{
// //    [TestFixture]
// //    public class EmbeddingGeneratorTests
// //    {
// //        private Mock<EmbeddingGenerator> _mockGenerator;
//
// //        [SetUp]
// //        public void SetUp()
// //        {
// //            _mockGenerator = new Mock<EmbeddingGenerator>();
// //            _mockGenerator.Setup(g => g.GenerateEmbeddings(It.IsAny<string>()))
// //                          .ReturnsAsync(new double[] { 0.1, 0.2, 0.3 });
// //        }
//
// //        [Test]
// //        public async Task GenerateEmbeddings_ShouldReturnValidEmbedding()
// //        {
// //            // Act
// //            double[] result = await _mockGenerator.Object.GenerateEmbeddings("test");
//
// //            // Assert
// //            Assert.That(result, Is.Not.Null);
// //            Assert.That(result.Length, Is.EqualTo(3));
// //        }
//
// //        [Test]
// //        public async Task GenerateEmbeddings_ShouldHandleEmptyString()
// //        {
// //            // Act
// //            double[] result = await _mockGenerator.Object.GenerateEmbeddings("");
//
// //            // Assert
// //            Assert.That(result, Is.Not.Null);
// //            Assert.That(result.Length, Is.EqualTo(3));
// //        }
//
// //        [Test]
// //        public async Task GenerateEmbeddings_ShouldHandleLongText()
// //        {
// //            // Act
// //            double[] result = await _mockGenerator.Object.GenerateEmbeddings(new string('A', 1000));
//
// //            // Assert
// //            Assert.That(result, Is.Not.Null);
// //        }
// //    }
// //}
//
// using GPTDocumentClustering.Services.Embedding;
// using NUnit.Framework;
// using NUnit.Framework.Legacy;
// using System.Linq;
//
// [TestFixture]
// public class EmbeddingGeneratorTests
// {
//     private EmbeddingGenerator _generator;
//
//     [SetUp]
//     public void Setup()
//     {
//         _generator = new EmbeddingGenerator();
//     }
//
//     [Test]
//     public async Task GenerateEmbeddings_ShouldReturnValidEmbedding()
//     {
//         var result = await _generator.GenerateEmbeddings("test");
//
//         
//        ClassicAssert.NotNull(result, "Embedding should not be null.");
//         Assert.That(result.Length, Is.EqualTo(3), "Embedding should have a fixed length.");
//     }
//
//     [Test]
//     public void GenerateEmbeddings_ShouldHandleEmptyString()
//     {
//         var result = _generator.GenerateEmbeddings("");
//
//         ClassicAssert.NotNull(result, "Even an empty string should return a valid embedding.");
//     }
//
//     [Test]
//     public void GenerateEmbeddings_ShouldHandleSpecialCharacters()
//     {
//         var result = _generator.GenerateEmbeddings("!@#$%^&*()_+");
//
//         ClassicAssert.NotNull(result, "Special characters should not break embedding generation.");
//     }
//
//     [Test]
//     public void GenerateEmbeddings_ShouldHandleLongText()
//     {
//         var longText = new string('A', 10000);
//         var result = _generator.GenerateEmbeddings(longText);
//
//         ClassicAssert.NotNull(result, "Long text should return an embedding.");
//     }
// }
