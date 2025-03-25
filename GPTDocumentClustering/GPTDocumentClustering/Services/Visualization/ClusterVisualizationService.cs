using System.Diagnostics;
using Accord.Statistics.Analysis;
using GPTDocumentClustering.Helper;
using GPTDocumentClustering.Models;
using ScottPlot;

namespace GPTDocumentClustering.Services.Visualization;

/// <summary>
/// Provides visualization services for document clusters
/// Responsible for creating visual representations of document embeddings and clusters
/// </summary>
public class ClusterVisualizationService
{
    private readonly List<Document> _documents;
    private readonly Dictionary<int, System.Drawing.Color> _clusterColors;
    private readonly Dictionary<string, System.Drawing.Color> _categoryColors;
    private Dictionary<int, String> ClusterToCategoryMap { get; }


    /// <summary>
    /// Constructor for ClusterVisualizationService
    /// Initializes color mappings and cluster-to-category mapping
    /// </summary>
    public ClusterVisualizationService(List<Document> documents)
    {
        _documents = documents;
        
        //Map Clusters to original category By max vote 
        ClusterToCategoryMap = ClusterToCategoryMapper();
        
        // Generate colors for clusters
        var clusterIds = documents.Select(d => d.ClusterId ?? -1).Distinct().ToList();
        _clusterColors = new Dictionary<int, System.Drawing.Color>();
        _categoryColors = new Dictionary<string, System.Drawing.Color>();
        
        for (int i = 0; i < clusterIds.Count; i++)
        {
            // Generate a color from HSL for better visual separation
            var hue = (float)i / clusterIds.Count;
            _clusterColors[clusterIds[i]] = ColorFromHsl(hue, 0.75f, 0.5f);
            _categoryColors[ClusterToCategoryMap[clusterIds[i]]] = ColorFromHsl(hue, 0.75f, 0.5f);
        }
        
        //Generate colors for categories
        var categories = documents.Select(d => d.Category).Distinct().ToList();
        _categoryColors = new Dictionary<string, System.Drawing.Color>();
        for (int i = 0; i < categories.Count; i++)
        {
            var hue = (float)(i + 0.5) / categories.Count; // Offset to differentiate from cluster colors
            _categoryColors[categories[i]] = ColorFromHsl(hue, 0.75f, 0.5f);
        }
        
    }

    public void AnalyzeAndVisualize(string outputFolder)
    {
        var points = ApplyPca();    // Dimensionality reduction
        Directory.CreateDirectory(outputFolder);
            
        // Generate visualizations
        VisualizeDocumentClusters(points, Path.Combine(outputFolder, "clusters.png"), false);
        VisualizeDocumentClusters(points, Path.Combine(outputFolder, "categories.png"), true);

        //CompareEmbeddingsBasedOnGroup(outputFolder + "/embeddingsVisualization");
        CreateLabeledHeatmap(Path.Combine(outputFolder, "heatmap.png"));
    }


    // Creates a heatmap visualization of embedding vectors
    private void CreateLabeledHeatmap(string outputPath)
    {
        int embeddingCount = _documents.Count;
        int dimensions = AppConstants.OpenAI.EmbeddingLength;
        
        var embeddings = _documents.Select(doc => doc.Embedding).ToList();
        var labels = _documents.Select(doc => doc.Category).ToList();

        // Sort embeddings by their category
        var sortedData = labels
            .Select((label, index) => new { Label = label, Embedding = embeddings[index] })
            .OrderBy(item => item.Label)
            .ToList();
        
        // Extract sorted embeddings and labels
        var sortedEmbeddings = sortedData.Select(item => item.Embedding).ToList();
        var sortedLabels = sortedData.Select(item => item.Label).ToList();
        
        // Create data array for heatmap
        double[,] heatmapData = new double[embeddingCount, dimensions];
        
        for (int i = 0; i < embeddingCount; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                heatmapData[i, j] = sortedEmbeddings[i][j];
            }
        }
        
        // Create the plot
        var plt = new Plot();
        
        // Add heatmap
        var hm = plt.Add.Heatmap(heatmapData);
        plt.Add.ColorBar(hm);
        
        // Label customization
        plt.Title("Embedding Vectors Heatmap by Label");
        plt.XLabel("Dimension Index");
        plt.YLabel("Embedding Index (Sorted by Label)");
        
        // Add label dividers and annotations
        string currentLabel = sortedLabels[0];
        int startIdx = 0;
        
        plt.Add.Annotation($"Label Top to Bottom:  \n{AppConstants.DataConstants.MyDictionary["3"]} " +
                           $"\n{AppConstants.DataConstants.MyDictionary["2"]} " +
                           $"\n{AppConstants.DataConstants.MyDictionary["1"]} " +
                           $"\n{AppConstants.DataConstants.MyDictionary["0"]}");
        for (int i = 1; i <= sortedLabels.Count; i++)
        {
            // If we've reached a new label or the end of the list, add a divider
            if (i == sortedLabels.Count || sortedLabels[i] != currentLabel)
            {
                // Add a horizontal line to separate labels
                plt.Add.HorizontalLine(i - 0.5, color : Color.FromColor(System.Drawing.Color.White));
                
                // Add label annotation
                //plt.Add.Annotation($"Label {currentLabel}", Alignment.MiddleCenter);
                // var anno = plt.Add.Annotation($"Label {currentLabel}");
                // anno.OffsetX = dimensions / (float)2.0;
                // anno.OffsetY = ((float)startIdx + i - 1) / (float)2.0;
                // anno.LabelFontSize = 16;
                // anno.LabelBackgroundColor = Color.FromColor(System.Drawing.Color.White);
                
                if (i < sortedLabels.Count)
                {
                    // Update for next label
                    currentLabel = sortedLabels[i];
                    startIdx = i;
                }
            }
        }
        
        // Save the plot
        plt.Save(outputPath, 1200, 800);
        Console.WriteLine($"Labeled heatmap saved to {outputPath}");

    }
    
    private void CompareEmbeddingsBasedOnGroup(string outputPath)
    {
        Console.WriteLine("Output path is : {0}", outputPath);
        
        var indices 
            = Enumerable
                .Range(1, AppConstants.OpenAI.EmbeddingLength)
                .Select(idx => (double)idx).ToArray();
        
        var categorizedDocs = _documents.GroupBy(d => d.Category).ToList();
        
        Console.WriteLine($"{categorizedDocs.Count} categorized documents");
        
        foreach (var group in categorizedDocs)
        {
            // Create a plot with appropriate size
            var plt = new Plot();
        
            foreach (var document in group)
            {
                Debug.Assert(document.Embedding != null, "document.Embedding != null");
                var scatter = plt.Add.ScatterPoints(
                    indices,
                    document.Embedding);
            }
        
            plt.Title($"Embeddings Visualization of Category {AppConstants.DataConstants.MyDictionary[group.Key]}");
            plt.ShowLegend();
            plt.Save(outputPath + $"_{group.Key}.png",1200, 800);
        }
        
    }
    
    private void CompareEmbeddings(string outputPath)
    {
        // Create a plot with appropriate size
        var plt = new Plot();
        var indices 
            = Enumerable
            .Range(1, AppConstants.OpenAI.EmbeddingLength)
            .Select(idx => (double)idx).ToArray();
        
        foreach (var document in _documents)
        {
            // Add each embedding with a different color and label
            Debug.Assert(document.Embedding != null, "document.Embedding != null");
            var scatter = plt.Add.ScatterPoints(
                indices,
                document.Embedding);
            scatter.MarkerSize = 2;
            scatter.LegendText = document.Category;
        }
        
        plt.Title("Embeddings Visualization");
        plt.ShowLegend();
        plt.Save(outputPath,1200, 800);
    }


    /// <summary>
    /// Create a color from HSL (Hue, Saturation, Lightness) color space
    /// Provides more visually distinct colors compared to RGB
    /// </summary>
    private System.Drawing.Color ColorFromHsl(float h, float s, float l)
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
                
            r = HueToRgb(p, q, h + 1.0f/3);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1.0f/3);
        }
            
        return System.Drawing.Color.FromArgb(
            (int)(r * 255),
            (int)(g * 255),
            (int)(b * 255)
        );
    }
        
    private float HueToRgb(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
            
        if (t < 1.0f/6) return p + (q - p) * 6 * t;
        if (t < 1.0f/2) return q;
        if (t < 2.0f/3) return p + (q - p) * (2.0f/3 - t) * 6;
            
        return p;
    }

    private void VisualizeDocumentClusters( List<Tuple<double,double>>? points, string outputPath, bool showCategories)
    {       
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
                var groupPoints = indices.Select(i =>
                {
                    Debug.Assert(points != null, nameof(points) + " != null");
                    return points[i];
                }).ToArray();
                var x = groupPoints.Select(p => p.Item1).ToArray();
                var y = groupPoints.Select(p => p.Item2).ToArray();
                    
                // Convert System.Drawing.Color to array of doubles for ScottPlot
                var scatter = plt.Add.ScatterPoints(x, y);
                scatter.LegendText = $"Category: {AppConstants.DataConstants.MyDictionary[group.Key]}";
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
                var groupPoints = indices.Select(i =>
                {
                    Debug.Assert(points != null, nameof(points) + " != null");
                    return points[i];
                }).ToArray();
                var x = groupPoints.Select(p => p.Item1).ToArray();
                var y = groupPoints.Select(p => p.Item2).ToArray();
                    
                // Convert System.Drawing.Color to ScottPlot format
                var scatter = plt.Add.ScatterPoints(x, y);
                scatter.LegendText = $"Cluster: {group.Key} = Category: {AppConstants.DataConstants.MyDictionary[ClusterToCategoryMap[group.Key]]}";
                scatter.MarkerSize = 7;
            }
            plt.Title("Document Visualization by Clusters");
            plt.Legend = new Legend(plt);
        }
            
        plt.ShowLegend();
        plt.Save(outputPath,800, 600);
    }

    private Dictionary<int, String> ClusterToCategoryMapper()
    {
        var clusters = _documents
            .Where(d => d.ClusterId.HasValue)
            .GroupBy(d =>
            {
                Debug.Assert(d.ClusterId != null);
                return d.ClusterId.Value;
            })
            .ToDictionary(g => g.Key, g => g.ToList());
        
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
    
    private List<Tuple<double, double>> ApplyPca()
    {
        //Extract embeddings as a matrix
        double[][] embeddings = _documents
            .Select(d => d.Embedding ?? [])
            .ToArray();
            
        // Check if we have valid embeddings
        if (embeddings.Length == 0 || embeddings[0].Length == 0)
        {
            throw new InvalidOperationException("Documents must have embeddings for visualization");
        }
            
        var reducedData = PrincipalComponentAnalysis.Reduce(embeddings, 2);
        return reducedData
            .Select(x => Tuple.Create(x[0], x[1]))
            .ToList();
    }
}