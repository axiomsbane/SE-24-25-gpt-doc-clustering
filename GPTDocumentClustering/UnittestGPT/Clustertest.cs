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

using NUnit.Framework;
using System.Collections.Generic;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Clustering;
using System.Linq;
using NUnit.Framework.Legacy;

[TestFixture]
public class ClusteringServiceTests
{
    private ClusteringService _clusteringService;

    [SetUp]
    public void Setup()
    {
        _clusteringService = new ClusteringService();
    }

    [Test]
    public void ClusterEmbeddings_ShouldAssignClusters_ToDocuments()
    {
        var documents = new List<Document>
        {
            new Document { SerialNo = 1, Embedding = new double[] { 0.1, 0.2, 0.3 } },
            new Document { SerialNo = 2, Embedding = new double[] { 0.5, 0.6, 0.7 } },
            new Document { SerialNo = 3, Embedding = new double[] { 0.9, 0.8, 0.7 } },
            new Document { SerialNo = 4, Embedding = new double[] { 0.2, 0.1, 0.3 } }
        };

        _clusteringService.ClusterEmbeddings(documents);

        Assert.That(documents.All(d => d.ClusterId != null), "All documents should have a ClusterId assigned.");
        Assert.That(documents.Select(d => d.ClusterId).Distinct().Count() > 1, "There should be multiple clusters.");
    }

    [Test]
    public void ClusterEmbeddings_ShouldHandleEmptyList()
    {
        var documents = new List<Document>();

        Assert.DoesNotThrow(() => _clusteringService.ClusterEmbeddings(documents), "Empty list should not cause errors.");
        ClassicAssert.IsEmpty(documents, "List should remain empty.");
    }

    [Test]
    public void ClusterEmbeddings_ShouldHandleSingleDocument()
    {
        var documents = new List<Document>
        {
            new Document { SerialNo = 1, Embedding = new double[] { 0.5, 0.6, 0.7 } }
        };

        _clusteringService.ClusterEmbeddings(documents);

        ClassicAssert.NotNull(documents[0].ClusterId, "Single document should still get a ClusterId.");
    }

    [Test]
    public void ClusterEmbeddings_ShouldHandleIdenticalEmbeddings()
    {
        var documents = new List<Document>
        {
            new Document { SerialNo = 1, Embedding = new double[] { 0.5, 0.5, 0.5 } },
            new Document { SerialNo = 2, Embedding = new double[] { 0.5, 0.5, 0.5 } },
            new Document { SerialNo = 3, Embedding = new double[] { 0.5, 0.5, 0.5 } }
        };

        _clusteringService.ClusterEmbeddings(documents);

        Assert.That(documents.Select(d => d.ClusterId).Distinct().Count(), Is.EqualTo(1), "Identical embeddings should have the same ClusterId.");
    }

    [Test]
    public void ClusterEmbeddings_ShouldHandleSparseVectors()
    {
        var documents = new List<Document>
        {
            new Document { SerialNo = 1, Embedding = new double[] { 0, 0, 1 } },
            new Document { SerialNo = 2, Embedding = new double[] { 0, 1, 0 } },
            new Document { SerialNo = 3, Embedding = new double[] { 1, 0, 0 } }
        };

        _clusteringService.ClusterEmbeddings(documents);

        Assert.That(documents.Select(d => d.ClusterId).Distinct().Count(), Is.EqualTo(3), "Sparse vectors should result in distinct clusters.");
    }
}
