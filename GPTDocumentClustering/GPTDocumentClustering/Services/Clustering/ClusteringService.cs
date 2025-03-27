
using Accord.MachineLearning;
using GPTDocumentClustering.Models;

namespace GPTDocumentClustering.Services.Clustering;
/// <summary>
/// This class is for running the clustering algorithm on the document
/// embeddings and assigning clusterIDs to each Document
/// </summary>
public class ClusteringService
{
    public void ClusterEmbeddings(List<Document> documents)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}