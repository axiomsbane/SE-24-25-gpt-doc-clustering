using GPTDocumentClustering.Models;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        public void VisualizeDocumentClusters(List<Tuple<double, double>>? points, string outputPath, bool showCategories = false)
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
            plt.Save(outputPath, 800, 600);
        }
    }
}