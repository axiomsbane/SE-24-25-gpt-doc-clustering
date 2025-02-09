using Accord.Statistics.Analysis;
using System.Numerics;
using System.Reflection.Metadata;

namespace GPTDocumentClustering.Services.Visualization;

public class ClusterVisualizationService
{
    public void VisualizeClusters(List<(Document Document, int ClusterIndex)> clusteredDcouments)
    {
        //var vectors = clusteredDocuments.Select(x => Vector<double>.Build.DenseOfArray(x.Document.Embedding)).ToArray();
        //var clusters = clusteredDocuments.Select(x => x.ClusterIndex).ToList();

        // Apply PCA - to reduced embedding in 2 Dimension
        var pca = new PrincipalComponentAnalysis();
        //var reducedData = pca.Reduce(vectors, 2);

        // Output coordinates
        Console.WriteLine("--- Reduced Document Coordinates ---");
        //for (int i = 0; i < reducedData.RowCount; i++)
        {
         //   Console.WriteLine($"Document {i}, Cluster {clusters[i]}: X = {reducedData[i, 0]:F4}, Y = {reducedData[i, 1]:F4}");
        }
        Console.WriteLine("--- End Reduced Document Coordinates ---");
    }
}