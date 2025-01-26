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

        // Method to process documents from file paths and return the cosine similarity between them
        public static void ProcessDocumentsFromFilePaths(string filePath1, string filePath2)
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
                Console.WriteLine($"Cosine Similarity: {similarity}");

                // Check if they are similar enough to be clustered together (using a default threshold of 0.75)
                bool areSimilar = IsSimilarEnough(vector1, vector2);
                Console.WriteLine($"Are Documents Similar Enough for Clustering: {areSimilar}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: File not found. {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        // Ask the user for file paths
        Console.WriteLine("Please enter the path to the first document:");
        string filePath1 = Console.ReadLine();

        Console.WriteLine("Please enter the path to the second document:");
        string filePath2 = Console.ReadLine();

        // Process the documents and compare them
        CosineSimilarityValidator.ProcessDocumentsFromFilePaths(filePath1, filePath2);
    }
}
