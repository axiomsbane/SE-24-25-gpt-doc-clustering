//using System.Collections.Generic;
//using System.Linq;
//using GPTDocumentClustering.Models;
//using GPTDocumentClustering.Services.Clustering;
//using NUnit.Framework;

//namespace GPTDocumentClustering.Tests
//{
//    [TestFixture]
//    public class ClusteringServiceTests
//    {
//        private ClusteringService _clusteringService;

//        [SetUp]
//        public void SetUp()
//        {
//            _clusteringService = new ClusteringService();
//        }

//        [Test]
//        public void ClusterEmbeddings_ShouldAssignClusters_ToDocuments()
//        {
//            // Arrange
//            var documents = new List<Document>
//            {
//                new Document { SerialNo = 1, Embedding = new double[] { 1.0, 2.0 } },
//                new Document { SerialNo = 2, Embedding = new double[] { 1.1, 2.1 } },
//                new Document { SerialNo = 3, Embedding = new double[] { 5.0, 6.0 } },
//                new Document { SerialNo = 4, Embedding = new double[] { 5.1, 6.1 } }
//            };

//            // Act
//            _clusteringService.ClusterEmbeddings(documents);

//            // Assert
//            Assert.That(documents.All(d => d.ClusterId != null), Is.True);
//            Assert.That(documents.Select(d => d.ClusterId).Distinct().Count(), Is.EqualTo(4));
//        }

//        [Test]
//        public void ClusterEmbeddings_ShouldHandleEmptyList()
//        {
//            // Arrange
//            var documents = new List<Document>();

//            // Act & Assert
//            Assert.DoesNotThrow(() => _clusteringService.ClusterEmbeddings(documents));
//            Assert.That(documents.Count, Is.EqualTo(0));
//        }

//        [Test]
//        public void ClusterEmbeddings_ShouldHandleSingleDocument()
//        {
//            // Arrange
//            var documents = new List<Document>
//            {
//                new Document { SerialNo = 1, Embedding = new double[] { 1.0, 2.0 } }
//            };

//            // Act
//            _clusteringService.ClusterEmbeddings(documents);

//            // Assert
//            Assert.That(documents.First().ClusterId, Is.Not.Null);
//        }
//    }
//}

