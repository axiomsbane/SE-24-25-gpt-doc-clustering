
using Accord.MachineLearning;
using GPTDocumentClustering.Models;

namespace GPTDocumentClustering.Services.Clustering;

public class ClusteringService
{
    public void ClusterEmbeddings(List<Document> documents)
    {
        Console.WriteLine("Cluster embeddings Part");
        double[]?[] embeddings = documents.Select(d => d.Embedding).ToArray();
        int k = 4; // Number of clusters 
        
        KMeans kmeans = new KMeans(k);
        KMeansClusterCollection clusters = kmeans.Learn(embeddings);
        
        // Assign each document to a cluster
        int[] labels = clusters.Decide(embeddings);
        for (int i = 0; i < labels.Length; i++)
        {
            documents[i].ClusterId = labels[i];
            Console.WriteLine($"Document {documents[i].SerialNo}: Cluster {labels[i]}");
        }
    }
}