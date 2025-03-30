using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Clustering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnittestGPT;

[TestClass]
public class ClusteringServiceTests
{
    [TestMethod]
    public void ClusterEmbeddings_ShouldAssignClusterIds()
    {
        // Arrange
        var documents = new List<Document>
        {
            new Document 
            { 
                SerialNo = 1, 
                Embedding = new double[] { 1.0, 2.0, 3.0 } 
            },
            new Document 
            { 
                SerialNo = 2, 
                Embedding = new double[] { 4.0, 5.0, 6.0 } 
            },
            new Document 
            { 
                SerialNo = 3, 
                Embedding = new double[] { 7.0, 8.0, 9.0 } 
            },
            new Document 
            { 
                SerialNo = 4, 
                Embedding = new double[] { 10.0, 11.0, 12.0 } 
            }
        };

        var clusteringService = new ClusteringService();

        // Act
        clusteringService.ClusterEmbeddings(documents);

        // Assert
        foreach (var doc in documents)
        {
            Assert.IsTrue(doc.ClusterId != null, $"Document {doc.SerialNo} should have a cluster ID");
            Assert.IsTrue(doc.ClusterId >= 0 && doc.ClusterId < 4, 
                $"Cluster ID for document {doc.SerialNo} should be between 0 and 3");
        }

        // Verify multiple clusters are used
        var uniqueClusterIds = new HashSet<int>(documents.Select(d => d.ClusterId??-1));
        Assert.IsTrue(uniqueClusterIds.Count > 1, "Documents should be assigned to multiple clusters");
    }

    [TestMethod]
    public void ClusterEmbeddings_EmptyDocumentList_ShouldNotThrowException()
    {
        // Arrange
        var documents = new List<Document>();
        var clusteringService = new ClusteringService();

        // Act & Assert
        try
        {
            clusteringService.ClusterEmbeddings(documents);
        }
        catch (Exception ex)
        {
            Assert.Fail($"ClusterEmbeddings should handle empty list without throwing an exception. Error: {ex.Message}");
        }
    }
}