using System.Collections.Generic;
using System.Linq;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Visualization;
using NUnit.Framework;

namespace GPTDocumentClustering.Tests
{
    [TestFixture]
    public class ClusterVisualizationServiceTests
    {
        [Test]
        public void ApplyPCA_ShouldReduceDimensions()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { SerialNo = 1, Embedding = new double[] { 1.0, 2.0, 3.0 } },
                new Document { SerialNo = 2, Embedding = new double[] { 4.0, 5.0, 6.0 } }
            };

            var service = new ClusterVisualizationService(documents);

            // Act
            var reducedPoints = service.ApplyPCA();

            // Assert
            Assert.That(reducedPoints.Count, Is.EqualTo(2));
            Assert.That(reducedPoints.All(p => p.Item1 is double), Is.True);
            Assert.That(reducedPoints.All(p => p.Item2 is double), Is.True);
        }
    }
}
