using System.Collections.Generic;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Validation;
using NUnit.Framework;
using static GPTDocumentClustering.Services.Validation.CosineSimilarityService;

namespace GPTDocumentClustering.Tests
{
    [TestFixture]
    public class CosineSimilarityServiceTests
    {
        [Test]
        public void EvaluateClusterQuality_ShouldReturnValidMetrics()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { SerialNo = 1, ClusterId = 0, Embedding = new double[] { 1.0, 0.0 } },
                new Document { SerialNo = 2, ClusterId = 0, Embedding = new double[] { 0.9, 0.1 } },
                new Document { SerialNo = 3, ClusterId = 1, Embedding = new double[] { 0.0, 1.0 } }
            };

            var service = new CosineSimilarityService(documents);

            // Act
            var metrics = service.EvaluateClusterQuality();

            // Assert
            Assert.That(metrics, Is.Not.Null);
            Assert.That(metrics.AverageIntraClusterSimilarity, Is.InRange(0, 1));
            Assert.That(metrics.AverageInterClusterSimilarity, Is.InRange(0, 1));
        }

        [Test]
        public void CosineSimilarity_ShouldReturnExpectedValue()
        {
            // Arrange
            var service = new CosineSimilarityService(new List<Document>());
            double[] vec1 = { 1, 0 };
            double[] vec2 = { 0, 1 };

            // Act
            double result = service.CosineSimilarity(vec1, vec2);

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(1e-5));
        }
    }
}

//using System.Collections.Generic;
//using GPTDocumentClustering.Models;
//using GPTDocumentClustering.Services.Validation;
//using NUnit.Framework;

//namespace GPTDocumentClustering.Tests
//{
//    [TestFixture]
//    public class CosineSimilarityServiceTests
//    {
//        private CosineSimilarityService _service;

//        [SetUp]
//        public void SetUp()
//        {
//            _service = new CosineSimilarityService(new List<Document>());
//        }

//        [Test]
//        public void CosineSimilarity_ShouldReturnOne_ForIdenticalVectors()
//        {
//            // Arrange
//            double[] vec1 = { 1, 1, 1 };
//            double[] vec2 = { 1, 1, 1 };

//            // Act
//            double result = _service.CosineSimilarity(vec1, vec2);

//            // Assert
//            Assert.That(result, Is.EqualTo(1).Within(1e-5));
//        }

//        [Test]
//        public void CosineSimilarity_ShouldReturnZero_ForOrthogonalVectors()
//        {
//            // Arrange
//            double[] vec1 = { 1, 0 };
//            double[] vec2 = { 0, 1 };

//            // Act
//            double result = _service.CosineSimilarity(vec1, vec2);

//            // Assert
//            Assert.That(result, Is.EqualTo(0).Within(1e-5));
//        }

//        [Test]
//        public void CosineSimilarity_ShouldThrowException_ForZeroVector()
//        {
//            // Arrange
//            double[] vec1 = { 0, 0, 0 };
//            double[] vec2 = { 1, 1, 1 };

//            // Act & Assert
//            Assert.Throws<System.DivideByZeroException>(() => _service.CosineSimilarity(vec1, vec2));
//        }
//    }
//}
