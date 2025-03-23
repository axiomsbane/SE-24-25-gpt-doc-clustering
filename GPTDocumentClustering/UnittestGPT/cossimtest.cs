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
            double result = service.CosineSimilarityService(vec1, vec2);

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(1e-5));
        }
    }
}