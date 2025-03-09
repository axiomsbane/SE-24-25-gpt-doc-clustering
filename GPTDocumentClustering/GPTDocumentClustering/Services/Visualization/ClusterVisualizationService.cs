using Accord.Statistics.Analysis;
using System.Numerics;
using System.Reflection.Metadata;

namespace GPTDocumentClustering.Services.Visualization;

// public class Document
// {
//     public double[] Embedding { get; set; }
// }
public class ClusterVisualizationService
{
    public void VisualizeClusters(List<(Document Document, int ClusterIndex)> document)
    {
        // Convert embeddings to double[][] format for Accord.NET
        // var vectors = document
        //     .Select(x => x.Document.Embedding)
        //     .ToArray();
        //
        // var clusters = document
        //     .Select(x => x.ClusterIndex)
        //     .ToList();
        //
        // // Apply PCA to reduce embeddings to 2 dimensions
        // var pca = new PrincipalComponentAnalysis()
        // {
        //     Method = PrincipalComponentMethod.Center
        // };
        //
        // // Compute the PCA
        // pca.Learn(vectors);
        //
        // // Transform the data
        // double[][] reducedData = pca.Transform(vectors);
        //
        // // Output coordinates
        // Console.WriteLine("--- Reduced Document Coordinates ---");
        // for (int i = 0; i < reducedData.Length; i++)
        // {
        //     Console.WriteLine($"Document {i}, Cluster {clusters[i]}: X = {reducedData[i][0]:F4}, Y = {reducedData[i][1]:F4}");
        // }
        // Console.WriteLine("--- End Reduced Document Coordinates ---");
    }
}