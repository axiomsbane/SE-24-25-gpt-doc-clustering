using NUnit.Framework;
using GPTDocumentClustering.Services.Validation;
using System;
using System.IO;

namespace GPTDocumentClustering.Tests
{
    [TestFixture]
    public class CosineSimilarityValidatorTests
    {
        [Test]
        public void CalculateCosineSimilarity_SameVectors_Returns1()
        {
            double[] vector1 = { 1, 2, 3 };
            double[] vector2 = { 1, 2, 3 };
            double similarity = CosineSimilarityValidator.CalculateCosineSimilarity(vector1, vector2);
            Assert.That(similarity, Is.EqualTo(1).Within(0.0001));
        }

        [Test]
        public void CalculateCosineSimilarity_OppositeVectors_ReturnsNeg1()
        {
            double[] vector1 = { 1, 2, 3 };
            double[] vector2 = { -1, -2, -3 };
            double similarity = CosineSimilarityValidator.CalculateCosineSimilarity(vector1, vector2);
            Assert.That(similarity, Is.EqualTo(-1).Within(0.0001));
        }

        [Test]
        public void CalculateCosineSimilarity_OrthogonalVectors_Returns0()
        {
            double[] vector1 = { 1, 0 };
            double[] vector2 = { 0, 1 };
            double similarity = CosineSimilarityValidator.CalculateCosineSimilarity(vector1, vector2);
            Assert.That(similarity, Is.EqualTo(0).Within(0.0001));
        }

        [Test]
        public void CalculateCosineSimilarity_DifferentLengths_ThrowsException()
        {
            double[] vector1 = { 1, 2, 3 };
            double[] vector2 = { 1, 2 };
            Assert.Throws<ArgumentException>(() => CosineSimilarityValidator.CalculateCosineSimilarity(vector1, vector2));
        }

        [Test]
        public void IsSimilarEnough_AboveThreshold_ReturnsTrue()
        {
            double[] vector1 = { 1, 2, 3 };
            double[] vector2 = { 1.1, 2.2, 3.3 }; 
            bool isSimilar = CosineSimilarityValidator.IsSimilarEnough(vector1, vector2, 0.75);
            Assert.IsTrue(isSimilar);
        }

        [Test]
        public void IsSimilarEnough_BelowThreshold_ReturnsFalse()
        {
            double[] vector1 = { 1, 0 };
            double[] vector2 = { 0, 1 };
            bool isSimilar = CosineSimilarityValidator.IsSimilarEnough(vector1, vector2, 0.75);
            Assert.IsFalse(isSimilar);
        }

        [Test]
        public void CreateDocumentVector_ValidDocument_ReturnsCorrectVector()
        {
            string document = "This is a test document. This document is a test.";
            HashSet<string> allWords = new HashSet<string> { "this", "is", "a", "test", "document" };
            double[] vector = CosineSimilarityValidator.CreateDocumentVector(document, allWords);
            double[] expectedVector = { 2, 2, 2, 2, 2 };
            Assert.That(vector, Is.EqualTo(expectedVector));
        }

        [Test]
        public void CreateDocumentVector_EmptyDocument_ReturnsZeroVector()
        {
            string document = "";
            HashSet<string> allWords = new HashSet<string> { "this", "is", "a", "test", "document" };
            double[] vector = CosineSimilarityValidator.CreateDocumentVector(document, allWords);
            double[] expectedVector = { 0, 0, 0, 0, 0 };
            Assert.That(vector, Is.EqualTo(expectedVector));
        }


        [Test]
        public void CalculateCosineSimilarityFromFiles_ValidFiles_ReturnsCorrectSimilarity()
        {
            string filePath1 = Path.GetTempFileName();
            string filePath2 = Path.GetTempFileName();
            File.WriteAllText(filePath1, "This is a test document.");
            File.WriteAllText(filePath2, "This document is a test.");

            double similarity = CosineSimilarityValidator.CalculateCosineSimilarityFromFiles(filePath1, filePath2);

            File.Delete(filePath1);
            File.Delete(filePath2);

            Assert.That(similarity, Is.EqualTo(1).Within(0.0001));
        }

        [Test]
        public void CalculateCosineSimilarityFromFiles_FileNotFound_ReturnsNeg1()
        {
            string filePath1 = "nonexistent_file1.txt";
            string filePath2 = "nonexistent_file2.txt";

            double similarity = CosineSimilarityValidator.CalculateCosineSimilarityFromFiles(filePath1, filePath2);

            Assert.That(similarity, Is.EqualTo(-1));
        }

        [Test]
        public void AreDocumentsSimilarForClustering_SimilarFiles_ReturnsTrue()
        {
            string filePath1 = Path.GetTempFileName();
            string filePath2 = Path.GetTempFileName();
            File.WriteAllText(filePath1, "This is a test document.");
            File.WriteAllText(filePath2, "This document is a test.");

            bool areSimilar = CosineSimilarityValidator.AreDocumentsSimilarForClustering(filePath1, filePath2);

            File.Delete(filePath1);
            File.Delete(filePath2);

            Assert.IsTrue(areSimilar);
        }

        [Test]
        public void AreDocumentsSimilarForClustering_DifferentFiles_ReturnsFalse()
        {
            string filePath1 = Path.GetTempFileName();
            string filePath2 = Path.GetTempFileName();
            File.WriteAllText(filePath1, "This is document 1.");
            File.WriteAllText(filePath2, "This is document 2.");

            bool areSimilar = CosineSimilarityValidator.AreDocumentsSimilarForClustering(filePath1, filePath2);

            File.Delete(filePath1);
            File.Delete(filePath2);

            Assert.IsFalse(areSimilar);
        }

        [Test]
        public void AreDocumentsSimilarForClustering_FileNotFound_ReturnsFalse()
        {
            string filePath1 = "nonexistent_file1.txt";
            string filePath2 = "nonexistent_file2.txt";

            bool areSimilar = CosineSimilarityValidator.AreDocumentsSimilarForClustering(filePath1, filePath2);

            Assert.IsFalse(areSimilar);
        }
    }
}