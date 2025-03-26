using System.Diagnostics;
using GPTDocumentClustering.Models;

namespace GPTDocumentClustering.Services.Validation;

public class CosineSimilarityService
{
    private readonly List<Document> _documents;
    
    public CosineSimilarityService(List<Document> documents)
    {
        _documents = documents;
    }

    public void Analyze(string outputFolder)
    {
        var metrics = EvaluateClusterQuality();
        File.WriteAllText(
            Path.Combine(outputFolder, "cluster_evaluation.txt"),
            FormatEvaluationResults(metrics)
        );
    }

    private ClusterEvaluationMetrics EvaluateClusterQuality()
        {
            var metrics = new ClusterEvaluationMetrics();
            
            // Calculate intra-cluster similarity (documents within the same cluster)
            var clusters = _documents
                .Where(d => d.ClusterId.HasValue)
                .GroupBy(d =>
                {
                    Debug.Assert(d.ClusterId != null);
                    return d.ClusterId.Value;
                })
                .ToDictionary(g => g.Key, g => g.ToList());
            
            double totalIntraSimilarity = 0;
            int totalIntraComparisons = 0;
            
            foreach (var cluster in clusters.Values)
            {
                double clusterSimilarity = 0;
                int comparisons = 0;
                
                for (int i = 0; i < cluster.Count; i++)
                {
                    for (int j = i + 1; j < cluster.Count; j++)
                    {
                        double similarity = CosineSimilarity(cluster[i].Embedding, cluster[j].Embedding);
                        clusterSimilarity += similarity;
                        comparisons++;
                    }
                }
                
                if (comparisons > 0)
                {
                    double avgClusterSimilarity = clusterSimilarity / comparisons;
                    Console.WriteLine($"Cluster similarity: {avgClusterSimilarity:P2}");
                    var clusterId = cluster[0].ClusterId;
                    if (clusterId != null)
                        metrics.IntraClusterSimilarities[clusters.Keys.ToList().IndexOf(clusterId.Value)] =
                            avgClusterSimilarity;
                    totalIntraSimilarity += clusterSimilarity;
                    totalIntraComparisons += comparisons;
                }
            }
            
            metrics.AverageIntraClusterSimilarity = totalIntraComparisons > 0 
                ? totalIntraSimilarity / totalIntraComparisons 
                : 0;
            
            // Calculate inter-cluster similarity (documents from different clusters)
            double totalInterSimilarity = 0;
            int totalInterComparisons = 0;
            
            var clusterKeys = clusters.Keys.ToList();
            for (int i = 0; i < clusterKeys.Count; i++)
            {
                for (int j = i + 1; j < clusterKeys.Count; j++)
                {
                    double clusterPairSimilarity = 0;
                    int comparisons = 0;
                    
                    foreach (var doc1 in clusters[clusterKeys[i]])
                    {
                        foreach (var doc2 in clusters[clusterKeys[j]])
                        {
                            double similarity = CosineSimilarity(doc1.Embedding, doc2.Embedding);
                            clusterPairSimilarity += similarity;
                            comparisons++;
                        }
                    }
                    
                    if (comparisons > 0)
                    {
                        totalInterSimilarity += clusterPairSimilarity;
                        totalInterComparisons += comparisons;
                    }
                }
            }
            
            metrics.AverageInterClusterSimilarity = totalInterComparisons > 0 
                ? totalInterSimilarity / totalInterComparisons 
                : 0;
            
            // Calculate categorical similarity (documents with the same original category)
            var categories = _documents.GroupBy(d => d.Category).ToDictionary(g => g.Key, g => g.ToList());
            
            double totalCategorySimilarity = 0;
            int totalCategoryComparisons = 0;
            
            foreach (var category in categories.Values)
            {
                double categorySimilarity = 0;
                int comparisons = 0;
                
                for (int i = 0; i < category.Count; i++)
                {
                    for (int j = i + 1; j < category.Count; j++)
                    {
                        double similarity = CosineSimilarity(category[i].Embedding, category[j].Embedding);
                        categorySimilarity += similarity;
                        comparisons++;
                    }
                }
                
                if (comparisons > 0)
                {
                    double avgCategorySimilarity = categorySimilarity / comparisons;
                    metrics.CategorySimilarities[categories.Keys.ToList().IndexOf(category[0].Category)] = avgCategorySimilarity;
                    totalCategorySimilarity += categorySimilarity;
                    totalCategoryComparisons += comparisons;
                }
            }
            
            metrics.AverageCategorySimilarity = totalCategoryComparisons > 0 
                ? totalCategorySimilarity / totalCategoryComparisons 
                : 0;
            
            // Calculate cluster purity and mapping
            metrics.ClusterToOriginalMapping = MapClustersToOriginalCategories(clusters);
            metrics.ClusterPurity = CalculateClusterPurity(clusters, metrics.ClusterToOriginalMapping);
            
            return metrics;
        }
    private Dictionary<int, string> MapClustersToOriginalCategories(
            Dictionary<int, List<Document>> clusters)
        {
            var mapping = new Dictionary<int, string>();
            
            foreach (var cluster in clusters)
            {
                var categoryDistribution = cluster.Value
                    .GroupBy(d => d.Category)
                    .ToDictionary(g => g.Key, g => g.Count());
                
                // Find the most common category in this cluster
                string dominantCategory = categoryDistribution
                    .OrderByDescending(kvp => kvp.Value)
                    .First().Key;
                
                mapping[cluster.Key] = dominantCategory;
            }
            
            return mapping;
        }
    private Dictionary<int, double> CalculateClusterPurity(
        Dictionary<int, List<Document>> clusters,
        Dictionary<int, string> mapping)
    {
        var purity = new Dictionary<int, double>();
            
        foreach (var cluster in clusters)
        {
            if (!mapping.ContainsKey(cluster.Key)) continue;
                
            string dominantCategory = mapping[cluster.Key];
            int documentsInDominantCategory = cluster.Value.Count(d => d.Category == dominantCategory);
            double clusterPurity = (double)documentsInDominantCategory / cluster.Value.Count;
                
            purity[cluster.Key] = clusterPurity;
        }
            
        return purity;
    }
    
    private string FormatEvaluationResults(ClusterEvaluationMetrics metrics)
        {
            var result = new System.Text.StringBuilder();
            
            result.AppendLine("=== Document Clustering Evaluation ===");
            result.AppendLine();
            
            result.AppendLine("Overall Metrics:");
            result.AppendLine($"Average Intra-Cluster Similarity: {metrics.AverageIntraClusterSimilarity:F4}");
            result.AppendLine($"Average Inter-Cluster Similarity: {metrics.AverageInterClusterSimilarity:F4}");
            result.AppendLine($"Average Category Similarity: {metrics.AverageCategorySimilarity:F4}");
            result.AppendLine();
            
            // Calculate silhouette coefficient (higher is better)
            double silhouette = (metrics.AverageIntraClusterSimilarity - metrics.AverageInterClusterSimilarity) / 
                               Math.Max(metrics.AverageIntraClusterSimilarity, metrics.AverageInterClusterSimilarity);
            result.AppendLine($"Silhouette Coefficient: {silhouette:F4} (higher is better)");
            result.AppendLine();
            
            result.AppendLine("Cluster to Category Mapping:");
            foreach (var mapping in metrics.ClusterToOriginalMapping)
            {
                result.AppendLine($"Cluster {mapping.Key} -> Category '{mapping.Value}' (Purity: {metrics.ClusterPurity[mapping.Key]:P2})");
            }
            result.AppendLine();
            
            result.AppendLine("Cluster Details:");
            foreach (var similarity in metrics.IntraClusterSimilarities)
            {
                result.AppendLine($"Cluster {similarity.Key} Intra-Similarity: {similarity.Value:F4}");
            }
            
            result.AppendLine();
            result.AppendLine("Category Details:");
            foreach (var similarity in metrics.CategorySimilarities)
            {
                result.AppendLine($"Category {similarity.Key} Intra-Similarity: {similarity.Value:F4}");
            }
            
            return result.ToString();
        }
    private double CosineSimilarity(double[]? v1, double[]? v2)
    {
        if (v1 == null || v2 == null || v1.Length != v2.Length)
            return 0;
            
        double dotProduct = 0;
        double magnitude1 = 0;
        double magnitude2 = 0;
            
        for (int i = 0; i < v1.Length; i++)
        {
            dotProduct += v1[i] * v2[i];
            magnitude1 += v1[i] * v1[i];
            magnitude2 += v2[i] * v2[i];
        }
            
        magnitude1 = Math.Sqrt(magnitude1);
        magnitude2 = Math.Sqrt(magnitude2);
            
        if (magnitude1 == 0 || magnitude2 == 0)
            return 0;
            
        return dotProduct / (magnitude1 * magnitude2);
    }
}