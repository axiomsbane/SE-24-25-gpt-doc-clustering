using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GPTDocumentClustering.Services.Validation;

namespace GPTDocumentClustering.Services.Validation
{
    public class CosineSimilarityValidator
    {
        // Method to calculate cosine similarity between two vectors
        public static double CalculateCosineSimilarity(double[] vector1, double[] vector2)
        {
            if (vector1.Length != vector2.Length)
                throw new ArgumentException("Vectors must have the same length.");

            // Dot product of vector1 and vector2
            double dotProduct = vector1.Zip(vector2, (v1, v2) => v1 * v2).Sum();

            // Magnitudes of vector1 and vector2
            double magnitude1 = Math.Sqrt(vector1.Sum(v => v * v));
            double magnitude2 = Math.Sqrt(vector2.Sum(v => v * v));

            // Cosine similarity
            return dotProduct / (magnitude1 * magnitude2);
        }

        // Method to determine if the cosine similarity is above a certain threshold for clustering
        public static bool IsSimilarEnough(double[] vector1, double[] vector2, double threshold = 0.75)
        {
            double similarity = CalculateCosineSimilarity(vector1, vector2);
            return similarity >= threshold;
        }

        // Method to create a word frequency vector for a document
        public static double[] CreateDocumentVector(string document, HashSet<string> allWords)
        {
            var words = document.Split(new[] { ' ', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.ToLower())
                .ToList();

            return allWords.Select(word => words.Count(w => w == word)).Select(count => (double)count).ToArray();
        }

        // Function to calculate the cosine similarity between two documents given their file paths
        public static double CalculateCosineSimilarityFromFiles(string filePath1, string filePath2)
        {
            try
            {
                string document1 = File.ReadAllText(filePath1);
                string document2 = File.ReadAllText(filePath2);

                HashSet<string> allWords = new HashSet<string>();
                allWords.UnionWith(document1.Split(new[] { ' ', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word.ToLower()));
                allWords.UnionWith(document2.Split(new[] { ' ', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word.ToLower()));

                double[] vector1 = CreateDocumentVector(document1, allWords);
                double[] vector2 = CreateDocumentVector(document2, allWords);

                return CalculateCosineSimilarity(vector1, vector2);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: File not found. {ex.Message}");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1;
            }
        }

        // Function to check if two documents are similar enough for clustering
        public static bool AreDocumentsSimilarForClustering(string filePath1, string filePath2, double threshold = 0.75)
        {
            double similarity = CalculateCosineSimilarityFromFiles(filePath1, filePath2);
            return similarity != -1 && similarity >= threshold;
        }

        // New method: Encapsulates the previous Main function logic
        public static void RunDocumentSimilarityCheck()
        {
            Console.WriteLine("Please enter the path to the first document:");
            string filePath1 = Console.ReadLine();

            Console.WriteLine("Please enter the path to the second document:");
            string filePath2 = Console.ReadLine();

            double similarity = CalculateCosineSimilarityFromFiles(filePath1, filePath2);
            Console.WriteLine($"Cosine Similarity: {similarity}");

            bool areSimilar = AreDocumentsSimilarForClustering(filePath1, filePath2);
            Console.WriteLine($"Are Documents Similar Enough for Clustering: {areSimilar}");
        }
    }
}

//using System;
//using System.Linq;
//using System.Collections.Generic;

//public class CosineSimilarityCalculator
//{
//    public static double CosineSimilarity(double[] embedding1, double[] embedding2)
//    {
//        if (embedding1.Length != embedding2.Length)
//            throw new ArgumentException("Embeddings must be of the same length");

//        double dotProduct = embedding1.Zip(embedding2, (a, b) => a * b).Sum();
//        double norm1 = Math.Sqrt(embedding1.Sum(a => a * a));
//        double norm2 = Math.Sqrt(embedding2.Sum(b => b * b));

//        return (norm1 == 0 || norm2 == 0) ? 0.0 : dotProduct / (norm1 * norm2);
//    }

//    public static double[] NormalizeVector(double[] embedding)
//    {
//        double norm = Math.Sqrt(embedding.Sum(a => a * a));
//        return norm == 0 ? embedding : embedding.Select(a => a / norm).ToArray();
//    }

//    public static double CosineSimilarityWithNormalization(double[] embedding1, double[] embedding2)
//    {
//        return CosineSimilarity(NormalizeVector(embedding1), NormalizeVector(embedding2));
//    }

//    public static double[] BatchCosineSimilarity(double[][] embeddings1, double[][] embeddings2)
//    {
//        if (embeddings1.Length != embeddings2.Length)
//            throw new ArgumentException("Batch sizes must be the same");

//        return embeddings1.Zip(embeddings2, (e1, e2) => CosineSimilarity(e1, e2)).ToArray();
//    }

//    public static bool IsSimilar(double[] embedding1, double[] embedding2, double threshold = 0.8)
//    {
//        return CosineSimilarity(embedding1, embedding2) >= threshold;
//    }
//}