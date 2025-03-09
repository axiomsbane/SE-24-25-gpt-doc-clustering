

using Accord.Statistics.Analysis;
using GPTDocumentClustering.Models;
using ScottPlot;

namespace GPTDocumentClustering.Services.Visualization;

public class ClusterVisualizationService
{
    private readonly List<Document> _documents;
    private readonly Dictionary<int, System.Drawing.Color> _clusterColors;
    private readonly Dictionary<string, System.Drawing.Color> _categoryColors;

    public ClusterVisualizationService(List<Document> documents)
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

    public void AnalyzeAndVisualize(string outputFolder)
    {
        var points = ApplyPCA();
        Directory.CreateDirectory(outputFolder);
            
        // Generate visualizations
        VisualizeDocumentClusters(points, Path.Combine(outputFolder, "clusters.png"), false);
        VisualizeDocumentClusters(points, Path.Combine(outputFolder, "categories.png"), true);
            
        // Generate side-by-side comparison
        //CreateComparisonVisualization(Path.Combine(outputFolder, "comparison.png"));
            
        // // Calculate and save evaluation metrics
        // var metrics = EvaluateClusterQuality();
        // File.WriteAllText(
        //     Path.Combine(outputFolder, "cluster_evaluation.txt"),
        //     FormatEvaluationResults(metrics)
        // );
    }
    
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
            
            var reducedData = PrincipalComponentAnalysis.Reduce(embeddings, 2);
            return reducedData
                .Select(x => Tuple.Create(x[0], x[1]))
                .ToList();;
        }
}