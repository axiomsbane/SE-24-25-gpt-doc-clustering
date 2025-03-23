using System.Collections.Generic;
using System.Linq;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Clustering;
using NUnit.Framework;

namespace GPTDocumentClustering.Tests
{
    [TestFixture]
    public class ClusteringServiceTests
    {
        [Test]
        public void ClusterEmbeddings_ShouldAssignClusters_ToDocuments()
        {
            // Arrange
            var documents = new List<Document>
            {
                new Document { SerialNo = 1, Embedding = new double[] { 1.0, 2.0 } },
                new Document { SerialNo = 2, Embedding = new double[] { 1.1, 2.1 } },
                new Document { SerialNo = 3, Embedding = new double[] { 5.0, 6.0 } },
                new Document { SerialNo = 4, Embedding = new double[] { 5.1, 6.1 } }
            };

            var clusteringService = new ClusteringService();

            // Act
            clusteringService.ClusterEmbeddings(documents);

            // Assert
            Assert.That(documents.All(d => d.ClusterId != null), Is.True);
            Assert.That(documents.Select(d => d.ClusterId).Distinct().Count(), Is.EqualTo(4));
        }
    }
}
