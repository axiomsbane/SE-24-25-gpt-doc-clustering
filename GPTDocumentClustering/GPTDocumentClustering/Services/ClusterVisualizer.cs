using GPTDocumentClustering.Models;
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
    }
}