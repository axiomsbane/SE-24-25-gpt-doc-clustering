using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics.Analysis;
using GPTDocumentClustering.Models;
using ScottPlot;
// For ScottPlot colors

// For System.Drawing.Color

namespace GPTDocumentClustering.Services
{
    public class ClusterVisualizer
    {
        private readonly List<Document> _documents;
        private readonly Dictionary<int, System.Drawing.Color> _clusterColors;
        private readonly Dictionary<string, System.Drawing.Color> _categoryColors;

        public ClusterVisualizer(List<Document> documents)
        {
            _documents = documents;
            
            // Generate colors for clusters
            var clusterIds = documents.Select(d => d.ClusterId ?? -1).Distinct().ToList();
            _clusterColors = new Dictionary<int, System.Drawing.Color>();
            for (int i = 0; i < clusterIds.Count; i++)
            {
                // Generate a color from HSL for better visual separation
                var hue = (float)i / clusterIds.Count;
                _clusterColors[clusterIds[i]] = ColorFromHSL(hue, 0.75f, 0.5f);
            }
            
            // Generate colors for categories
            var categories = documents.Select(d => d.Category).Distinct().ToList();
            _categoryColors = new Dictionary<string, System.Drawing.Color>();
            for (int i = 0; i < categories.Count; i++)
            {
                var hue = (float)(i + 0.5) / categories.Count; // Offset to differentiate from cluster colors
                _categoryColors[categories[i]] = ColorFromHSL(hue, 0.75f, 0.5f);
            }
        }

        /// <summary>
        /// Visualizes document clusters using PCA for dimensionality reduction
        /// </summary>
        /// <param name="outputPath">Path to save the visualization image</param>
        /// <param name="showCategories">If true, color by categories instead of clusters</param>
        public void VisualizeDocumentClusters( List<Tuple<double,double>>? points, string outputPath, bool showCategories = false)
        {
            // Apply PCA to reduce dimensionality to 2D
            
            
            // Create a new plot
            var plt = new Plot();
            
            // Group documents by cluster or category for visualization
            if (showCategories)
            {
                // Group by original categories
                var categorizedDocs = _documents.GroupBy(d => d.Category).ToList();
                foreach (var group in categorizedDocs)
                {
                    var indices = group.Select(d => _documents.IndexOf(d)).ToArray();
                    var groupPoints = indices.Select(i => points[i]).ToArray();
                    var x = groupPoints.Select(p => p.Item1).ToArray();
                    var y = groupPoints.Select(p => p.Item2).ToArray();
                    
                    var color = _categoryColors[group.Key];
                    // Convert System.Drawing.Color to array of doubles for ScottPlot
                    var scatter = plt.Add.Scatter(x, y);
                    scatter.Label = $"Category: {group.Key}";
                    scatter.MarkerSize = 7;
                    plt.Legend = new Legend(plt);
                }
                plt.Title("Document Visualization by Original Categories");
            }
            else
            {
                // Group by clusters
                var clusteredDocs = _documents.GroupBy(d => d.ClusterId ?? -1).ToList();
                foreach (var group in clusteredDocs)
                {
                    var indices = group.Select(d => _documents.IndexOf(d)).ToArray();
                    var groupPoints = indices.Select(i => points[i]).ToArray();
                    var x = groupPoints.Select(p => p.Item1).ToArray();
                    var y = groupPoints.Select(p => p.Item2).ToArray();
                    
                    var color = _clusterColors[group.Key];
                    // Convert System.Drawing.Color to ScottPlot format
                    var scatter = plt.Add.Scatter(x, y);
                    scatter.Label = $"Cluster: {group.Key}";
                    scatter.MarkerSize = 7;
                }
                plt.Title("Document Visualization by Clusters");
                plt.Legend = new Legend(plt);
            }
            
            plt.ShowLegend();
            plt.Save(outputPath,800, 600);
        }

        /// <summary>
        /// Generate both visualizations and display cluster evaluation metrics
        /// </summary>
        /// <param name="outputFolder">Folder to save the visualizations</param>
        public void AnalyzeAndVisualize(string outputFolder)
        {
            var points = ApplyPCA();
            Directory.CreateDirectory(outputFolder);
            
            // Generate visualizations
            VisualizeDocumentClusters(points, Path.Combine(outputFolder, "clusters.png"), false);
            VisualizeDocumentClusters(points, Path.Combine(outputFolder, "categories.png"), true);
            
            // Generate side-by-side comparison
            //CreateComparisonVisualization(Path.Combine(outputFolder, "comparison.png"));
            
            // Calculate and save evaluation metrics
            var metrics = EvaluateClusterQuality();
            File.WriteAllText(
                Path.Combine(outputFolder, "cluster_evaluation.txt"),
                FormatEvaluationResults(metrics) 
            );
        }

        /// <summary>
        /// Creates a side-by-side visualization comparing clusters to original categories
        /// </summary>
        // private void CreateComparisonVisualization(string outputPath)
        // {
        //     var points = ApplyPCA();
        //     
        //     // Create a plot with two subplots
        //     var plt = new Multiplot();
        //     plt.Subplots.GetPlot(2).Title("Comparison: Original Categories vs. Clusters"););
        //     
        //     var subplots = plt.Subplots.GetPlot(2);
        //     
        //     // Left plot - categories
        //     var categorizedDocs = _documentsgit.GroupBy(d => d.Category).ToList();
        //     foreach (var group in categorizedDocs)
        //     {
        //         var indices = group.Select(d => _documents.IndexOf(d)).ToArray();
        //         var groupPoints = indices.Select(i => points[i]).ToArray();
        //         var x = groupPoints.Select(p => p.Item1).ToArray();
        //         var y = groupPoints.Select(p => p.Item2).ToArray();
        //         
        //         var color = _categoryColors[group.Key];
        //         var scatter = subplots[0, 0].AddScatter(x, y, ToScottPlotColor(color));
        //         scatter.Label = $"Category: {group.Key}";
        //         scatter.MarkerSize = 7;
        //     }
        //     subplots[0, 0].Title("Original Categories");
        //     
        //     // Right plot - clusters
        //     var clusteredDocs = _documents.GroupBy(d => d.ClusterId ?? -1).ToList();
        //     foreach (var group in clusteredDocs)
        //     {
        //         var indices = group.Select(d => _documents.IndexOf(d)).ToArray();
        //         var groupPoints = indices.Select(i => points[i]).ToArray();
        //         var x = groupPoints.Select(p => p.Item1).ToArray();
        //         var y = groupPoints.Select(p => p.Item2).ToArray();
        //         
        //         var color = _clusterColors[group.Key];
        //         var scatter = subplots[0, 1].AddScatter(x, y, ToScottPlotColor(color));
        //         scatter.Label = $"Cluster: {group.Key}";
        //         scatter.MarkerSize = 7;
        //     }
        //     subplots[0, 1].Title("Identified Clusters");
        //     
        //     plt.SaveFig(outputPath);
        // }

        /// <summary>
        /// Helper method to convert System.Drawing.Color to ScottPlot color format
        /// </summary>
        private System.Drawing.Color ToScottPlotColor(System.Drawing.Color color)
        {
            // Return the System.Drawing.Color as is since ScottPlot can use it directly
            return color;
        }

        /// <summary>
        /// Apply PCA to reduce document embeddings to 2D
        /// </summary>
        /// <returns>List of 2D points</returns>
        private List<Tuple<double, double>> ApplyPCA()
        {
            //Extract embeddings as a matrix
            double[][] embeddings = _documents
                .Select(d => d.Embedding ?? new double[0])
                .ToArray();
            
            // Check if we have valid embeddings
            if (embeddings.Length == 0 || embeddings[0].Length == 0)
            {
                throw new InvalidOperationException("Documents must have embeddings for visualization");
            }
            //
            // // Center the data
            // double[] means = Matrix.Mean(embeddings, 0);
            // for (int i = 0; i < embeddings.Length; i++)
            // {
            //     for (int j = 0; j < embeddings[i].Length; j++)
            //     {
            //         embeddings[i][j] -= means[j];
            //     }
            // }
            //
            // // Compute the covariance matrix
            // double[][] cov = Matrix.Covariance(embeddings);
            //
            // // Apply Singular Value Decomposition (SVD) for PCA
            // var svd = new SingularValueDecomposition(cov);
            // double[][] components = svd.RightSingularVectors;
            //
            // // Take the first two principal components
            // double[][] projection = new double[2][];
            // projection[0] = components.GetColumn(0);
            // projection[1] = components.GetColumn(1);
            //
            // // Project the embeddings onto the first two principal components
            // var result = new List<Tuple<double, double>>();
            // for (int i = 0; i < embeddings.Length; i++)
            // {
            //     double x = 0, y = 0;
            //     for (int j = 0; j < embeddings[i].Length; j++)
            //     {
            //         x += embeddings[i][j] * projection[0][j];
            //         y += embeddings[i][j] * projection[1][j];
            //     }
            //     result.Add(new Tuple<double, double>(x, y));
            // }
            // var vectors = _documents.Select(x => Vector<float>.Build.DenseOfArray(x.Document.Embedding)).ToArray();
            var reducedData = PrincipalComponentAnalysis.Reduce(embeddings, 2);
            return reducedData
                .Select(x => Tuple.Create(x[0], x[1]))
                .ToList();;
        }

        /// <summary>
        /// Evaluates the quality of clustering using cosine similarity
        /// </summary>
        public ClusterEvaluationMetrics EvaluateClusterQuality()
        {
            var metrics = new ClusterEvaluationMetrics();
            
            // Calculate intra-cluster similarity (documents within the same cluster)
            var clusters = _documents
                .Where(d => d.ClusterId.HasValue)
                .GroupBy(d => d.ClusterId.Value)
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
                    metrics.IntraClusterSimilarities[clusters.Keys.ToList().IndexOf(cluster[0].ClusterId.Value)] = avgClusterSimilarity;
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
            metrics.ClusterToOriginalMapping = MapClustersToOriginalCategories(clusters, categories);
            metrics.ClusterPurity = CalculateClusterPurity(clusters, metrics.ClusterToOriginalMapping);
            
            return metrics;
        }
        
        /// <summary>
        /// Maps clusters to their most likely original categories
        /// </summary>
        private Dictionary<int, string> MapClustersToOriginalCategories(
            Dictionary<int, List<Document>> clusters,
            Dictionary<string, List<Document>> categories)
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
        
        /// <summary>
        /// Calculates the purity of each cluster (% of dominant category)
        /// </summary>
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
        
        /// <summary>
        /// Calculates cosine similarity between two embeddings
        /// </summary>
        private double CosineSimilarity(double[] v1, double[] v2)
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
        
        /// <summary>
        /// Formats the evaluation metrics into a readable string
        /// </summary>
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
        
        /// <summary>
        /// Converts HSL values to RGB Color
        /// </summary>
        private System.Drawing.Color ColorFromHSL(float h, float s, float l)
        {
            float r, g, b;
            
            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                
                r = HueToRGB(p, q, h + 1.0f/3);
                g = HueToRGB(p, q, h);
                b = HueToRGB(p, q, h - 1.0f/3);
            }
            
            return System.Drawing.Color.FromArgb(
                (int)(r * 255),
                (int)(g * 255),
                (int)(b * 255)
            );
        }
        
        private float HueToRGB(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            
            if (t < 1.0f/6) return p + (q - p) * 6 * t;
            if (t < 1.0f/2) return q;
            if (t < 2.0f/3) return p + (q - p) * (2.0f/3 - t) * 6;
            
            return p;
        }
    }
    
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
}