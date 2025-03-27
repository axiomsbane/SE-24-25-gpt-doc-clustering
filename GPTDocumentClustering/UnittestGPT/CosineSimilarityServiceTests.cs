using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnittestGPT
{
    [TestClass]
    public class CosineSimilarityServiceTests
    {
        private List<Document> _documents;

        [TestInitialize]
        public void Setup()
        {
            _documents = new List<Document>
            {
                new Document { ClusterId = 1, Category = "A", Embedding = new double[] { 1, 0 } },
                new Document { ClusterId = 1, Category = "A", Embedding = new double[] { 2, 0 } },
                new Document { ClusterId = 2, Category = "B", Embedding = new double[] { 0, 1 } },
                new Document { ClusterId = 2, Category = "C", Embedding = new double[] { 0, 2 } },
                new Document { ClusterId = 3, Category = "B", Embedding = new double[] { 1, 1 } }
            };
        }

        [TestMethod]
        public void Constructor_InitializesWithDocuments()
        {
            // Arrange
            // Act
            var service = new CosineSimilarityService(_documents);

            // Assert - No direct way to assert private field, but subsequent calls will use it.
            Assert.IsNotNull(service);
        }
        
        

        [TestMethod]
        public void MapClustersToOriginalCategories_ReturnsCorrectMapping()
        {
            // Arrange
            var service = new CosineSimilarityService(_documents);
            var clusters = _documents
                .Where(d => d.ClusterId.HasValue)
                .GroupBy(d => d.ClusterId.Value)
                .ToDictionary(g => g.Key, g => g.ToList());
            var expectedMapping = new Dictionary<int, string>
            {
                { 1, "A" },
                { 2, "B" }, // B has count 1, C has count 1, B appears first
                { 3, "B" }
            };

            // Act
            var actualMapping = service.GetType().GetMethod("MapClustersToOriginalCategories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(service, new object[] { clusters }) as Dictionary<int, string>;

            // Assert
            Assert.IsNotNull(actualMapping);
            Assert.AreEqual(expectedMapping.Count, actualMapping.Count);
            foreach (var kvp in expectedMapping)
            {
                Assert.IsTrue(actualMapping.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, actualMapping[kvp.Key]);
            }
        }

        [TestMethod]
        public void CalculateClusterPurity_ReturnsCorrectPurity()
        {
            // Arrange
            var service = new CosineSimilarityService(_documents);
            var clusters = _documents
                .Where(d => d.ClusterId.HasValue)
                .GroupBy(d => d.ClusterId.Value)
                .ToDictionary(g => g.Key, g => g.ToList());
            var mapping = new Dictionary<int, string>
            {
                { 1, "A" },
                { 2, "B" },
                { 3, "B" }
            };
            var expectedPurity = new Dictionary<int, double>
            {
                { 1, 1.0 }, // 2 documents in cluster 1, both category A
                { 2, 0.5 }, // 2 documents in cluster 2, 1 category B, 1 category C (B is mapped)
                { 3, 1.0 }  // 1 document in cluster 3, category B
            };

            // Act
            var actualPurity = service.GetType().GetMethod("CalculateClusterPurity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(service, new object[] { clusters, mapping }) as Dictionary<int, double>;

            // Assert
            Assert.IsNotNull(actualPurity);
            Assert.AreEqual(expectedPurity.Count, actualPurity.Count);
            foreach (var kvp in expectedPurity)
            {
                Assert.IsTrue(actualPurity.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, actualPurity[kvp.Key], 0.001);
            }
        }
        

        [TestMethod]
        public void CosineSimilarity_ValidVectors_ReturnsCorrectSimilarity()
        {
            // Arrange
            var service = new CosineSimilarityService(_documents);
            double[] v1 = { 1, 2, 3 };
            double[] v2 = { 4, 5, 6 };
            double expectedSimilarity = (1 * 4 + 2 * 5 + 3 * 6) / (System.Math.Sqrt(1 * 1 + 2 * 2 + 3 * 3) * System.Math.Sqrt(4 * 4 + 5 * 5 + 6 * 6));

            // Act
            var actualSimilarity = service.GetType().GetMethod("CosineSimilarity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(service, new object[] { v1, v2 });

            // Assert
            Assert.IsNotNull(actualSimilarity);
            Assert.AreEqual(expectedSimilarity, (double)actualSimilarity, 0.0001);
        }

        [TestMethod]
        public void CosineSimilarity_NullVectors_ReturnsZero()
        {
            // Arrange
            var service = new CosineSimilarityService(_documents);
            double[] v1 = { 1, 2, 3 };
            double[] v2 = null;

            // Act
            var actualSimilarity = service.GetType().GetMethod("CosineSimilarity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(service, new object[] { v1, v2 });

            // Assert
            Assert.AreEqual(0.0, actualSimilarity);
        }

        [TestMethod]
        public void CosineSimilarity_DifferentLengthVectors_ReturnsZero()
        {
            // Arrange
            var service = new CosineSimilarityService(_documents);
            double[] v1 = { 1, 2, 3 };
            double[] v2 = { 4, 5 };

            // Act
            var actualSimilarity = service.GetType().GetMethod("CosineSimilarity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(service, new object[] { v1, v2 });

            // Assert
            Assert.AreEqual(0.0, actualSimilarity);
        }

        [TestMethod]
        public void CosineSimilarity_ZeroMagnitudeVectors_ReturnsZero()
        {
            // Arrange
            var service = new CosineSimilarityService(_documents);
            double[] v1 = { 0, 0, 0 };
            double[] v2 = { 4, 5, 6 };

            // Act
            var actualSimilarity = service.GetType().GetMethod("CosineSimilarity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(service, new object[] { v1, v2 });

            // Assert
            Assert.AreEqual(0.0, actualSimilarity);
        }
    }

    // Define interfaces for dependencies to enable mocking
    public interface IFileSystemWrapper
    {
        void WriteAllText(string path, string contents);
    }

    public interface IPathWrapper
    {
        string Combine(string path1, string path2);
    }
}