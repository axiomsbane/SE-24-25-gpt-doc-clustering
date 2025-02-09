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
            // Calculate the cosine similarity
            double similarity = CalculateCosineSimilarity(vector1, vector2);

            // Check if similarity exceeds the threshold
            return similarity >= threshold;
        }

        // Method to create a word frequency vector for a document
        public static double[] CreateDocumentVector(string document, HashSet<string> allWords)
        {
            // Split document into words and count their frequency
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
                // Read the contents of the documents from the file paths
                string document1 = File.ReadAllText(filePath1);
                string document2 = File.ReadAllText(filePath2);

                // Collect all unique words from both documents
                HashSet<string> allWords = new HashSet<string>();

                allWords.UnionWith(document1.Split(new[] { ' ', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word.ToLower()));

                allWords.UnionWith(document2.Split(new[] { ' ', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word.ToLower()));

                // Create vectors for both documents
                double[] vector1 = CreateDocumentVector(document1, allWords);
                double[] vector2 = CreateDocumentVector(document2, allWords);

                // Calculate the cosine similarity
                double similarity = CalculateCosineSimilarity(vector1, vector2);

                return similarity;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: File not found. {ex.Message}");
                return -1; // Return a negative value to indicate an error
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1; // Return a negative value to indicate an error
            }
        }

        // Function to check if two documents are similar enough for clustering
        public static bool AreDocumentsSimilarForClustering(string filePath1, string filePath2, double threshold = 0.75)
        {
            double similarity = CalculateCosineSimilarityFromFiles(filePath1, filePath2);

            if (similarity == -1)
            {
                return false; // If there's an error in reading files or calculating similarity
            }

            return similarity >= threshold;
        }
    }
}

class Program
{
    //static void Main()
    //{
    //    // Example usage: you can call this method whenever you need to calculate cosine similarity or check similarity
    //    Console.WriteLine("Please enter the path to the first document:");
    //    string filePath1 = Console.ReadLine();

    //    Console.WriteLine("Please enter the path to the second document:");
    //    string filePath2 = Console.ReadLine();

    //    // Calculate Cosine Similarity between the two documents
    //    double similarity = CosineSimilarityValidator.CalculateCosineSimilarityFromFiles(filePath1, filePath2);
    //    Console.WriteLine($"Cosine Similarity: {similarity}");

    //    // Check if the documents are similar enough for clustering (default threshold is 0.75)
    //    bool areSimilar = CosineSimilarityValidator.AreDocumentsSimilarForClustering(filePath1, filePath2);
    //    Console.WriteLine($"Are Documents Similar Enough for Clustering: {areSimilar}");
    //}
}