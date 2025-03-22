namespace GPTDocumentClustering.Services.Validation;

/// <summary>
/// Holds metrics for evaluating cluster quality
/// </summary>
public class ClusterEvaluationMetrics
{
    // Average similarity between documents in the same cluster
    public double AverageIntraClusterSimilarity { get; set; }
        
    // Average similarity between documents in different clusters
    public double AverageInterClusterSimilarity { get; set; }
        
    // Average similarity between documents with the same original category
    public double AverageCategorySimilarity { get; set; }

    // Similarity within each cluster
    public Dictionary<int, double> IntraClusterSimilarities { get; } = new Dictionary<int, double>();

    // Similarity within each category
    public Dictionary<int, double> CategorySimilarities { get; } = new Dictionary<int, double>();

    // Mapping from cluster IDs to original categories
    public Dictionary<int, string> ClusterToOriginalMapping { get; set; } = new Dictionary<int, string>();

    // Purity of each cluster (percentage of documents that match the dominant category)
    public Dictionary<int, double> ClusterPurity { get; set; } = new Dictionary<int, double>();
}