using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Visualization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ScottPlot;

namespace UnittestGPT
{
    [TestClass]
    public class ClusterVisualizationServiceTests
    {
        private List<Document> _documents;

        [TestInitialize]
        public void Setup()
        {
            _documents = new List<Document>
            {
                new Document { ClusterId = 1, Category = "CategoryA", Embedding = new double[] { 0.1, 0.2 } },
                new Document { ClusterId = 1, Category = "CategoryA", Embedding = new double[] { 0.3, 0.4 } },
                new Document { ClusterId = 2, Category = "CategoryB", Embedding = new double[] { 0.5, 0.6 } },
                new Document { ClusterId = 2, Category = "CategoryC", Embedding = new double[] { 0.7, 0.8 } },
                new Document { ClusterId = 3, Category = "CategoryB", Embedding = new double[] { 0.9, 1.0 } }
            };
        }

        [TestMethod]
        public void ClusterToCategoryMapper_ReturnsCorrectMapping()
        {
            // Arrange
            var service = new ClusterVisualizationService(_documents);
            var expectedMapping = new Dictionary<int, string>
            {
                { 1, "CategoryA" },
                { 2, "CategoryB" }, // CategoryB appears once, CategoryC appears once - order might vary, but B is first in list
                { 3, "CategoryB" }
            };

            // Act
            var actualMapping = service.GetType().GetMethod("ClusterToCategoryMapper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(service, null) as Dictionary<int, string>;

            // Assert
            Assert.IsNotNull(actualMapping);
            Assert.AreEqual(expectedMapping.Count, actualMapping.Count);
            foreach (var kvp in expectedMapping)
            {
                Assert.IsTrue(actualMapping.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, actualMapping[kvp.Key]);
            }
        }

        // Helper method to create System.Drawing.Color from ScottPlot.Color
        private static System.Drawing.Color ConvertColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
    
}