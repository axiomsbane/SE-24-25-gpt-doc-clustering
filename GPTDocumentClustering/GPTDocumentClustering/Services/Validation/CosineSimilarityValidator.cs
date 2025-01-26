using System;
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

        // Method to process input documents and return the cosine similarity between them
        public static void ProcessDocumentsAndCompare()
        {
            // Collect two documents from the user
            Console.WriteLine("Please enter the first document:");
            string document1 = Console.ReadLine();

            Console.WriteLine("Please enter the second document:");
            string document2 = Console.ReadLine();

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
    }
}

class Program
{
    static void Main()
    {
        // Run the process to compare two user-input documents
        CosineSimilarityValidator.ProcessDocumentsAndCompare();
    }
}
