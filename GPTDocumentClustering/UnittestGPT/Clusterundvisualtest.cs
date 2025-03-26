// using System.Collections.Generic;
// using System.Linq;
// using GPTDocumentClustering.Models;
// using GPTDocumentClustering.Services.Visualization;
// using NUnit.Framework;
// using NUnit.Framework.Legacy;
//
// namespace GPTDocumentClustering.Tests
// {
//     [TestFixture]
//     public class ClusterVisualizationServiceTests
//     {
//         [Test]
//         public async Task ApplyPCA_ShouldReduceDimensions()
//         {
//             // Arrange
//             var documents = new List<Document>
//             {
//                 new Document { SerialNo = 1, Embedding = new double[] { 1.0, 2.0, 3.0 } },
//                 new Document { SerialNo = 2, Embedding = new double[] { 4.0, 5.0, 6.0 } }
//             };
//
//             var service = new ClusterVisualizationService(documents);
//
//             // Act
//             var reducedPoints = await service.ApplyPca();
//
//             // Assert
//             ClassicAssert.That(reducedPoints, Is.EqualTo(2));
//             Assert.That(reducedPoints.All(p => p.Item1 is double), Is.True);
//             Assert.That(reducedPoints.All(p => p.Item2 is double), Is.True);
//         }
//     }
// }
//
// //using System.Collections.Generic;
// //using System.Linq;
// //using GPTDocumentClustering.Models;
// //using GPTDocumentClustering.Services.Visualization;
// //using NUnit.Framework;
//
// //namespace GPTDocumentClustering.Tests
// //{
// //    [TestFixture]
// //    public class ClusterVisualizationServiceTests
// //    {
// //        private ClusterVisualizationService _service;
//
// //        [Test]
// //        public void ApplyPCA_ShouldReduceToTwoDimensions()
// //        {
// //            // Arrange
// //            var documents = new List<Document>
// //            {
// //                new Document { SerialNo = 1, Embedding = new double[] { 1.0, 2.0, 3.0 } },
// //                new Document { SerialNo = 2, Embedding = new double[] { 4.0, 5.0, 6.0 } }
// //            };
//
// //            _service = new ClusterVisualizationService(documents);
//
// //            // Act
// //            var reducedPoints = _service.ApplyPCA();
//
// //            // Assert
// //            Assert.That(reducedPoints.Count, Is.EqualTo(2));
// //            Assert.That(reducedPoints.All(p => p.Item1 is double && p.Item2 is double), Is.True);
// //        }
//
// //        [Test]
// //        public void ApplyPCA_ShouldHandleSinglePoint()
// //        {
// //            // Arrange
// //            var documents = new List<Document>
// //            {
// //                new Document { SerialNo = 1, Embedding = new double[] { 1.0, 2.0, 3.0 } }
// //            };
//
// //            _service = new ClusterVisualizationService(documents);
//
// //            // Act
// //            var reducedPoints = _service.ApplyPCA();
//
// //            // Assert
// //            Assert.That(reducedPoints.Count, Is.EqualTo(1));
// //        }
//
// //        [Test]
// //        public void ApplyPCA_ShouldHandleEmptyList()
// //        {
// //            // Arrange
// //            var documents = new List<Document>();
// //            _service = new ClusterVisualizationService(documents);
//
// //            // Act
// //            var reducedPoints = _service.ApplyPCA();
//
// //            // Assert
// //            Assert.That(reducedPoints.Count, Is.EqualTo(0));
// //        }
// //    }
// //}
