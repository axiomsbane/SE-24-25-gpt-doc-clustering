using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using GPTDocumentClustering.Helper;
using GPTDocumentClustering.Interfaces.InputData;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Clustering;
using GPTDocumentClustering.Services.Embedding;
using GPTDocumentClustering.Services.InputData;
using GPTDocumentClustering.Services.Visualization;
using OpenAI.Embeddings;

namespace GPTDocumentClustering;

class Program
{
    static async Task Main(string[] args)
    {
        
        // TestMethod();
        // return;
        
        var dataReaderService = new CsvDataReader(Environment.GetEnvironmentVariable("INPUT_FILE_PATH"));
        var embeddingService = new EmbeddingService();
        // var visualizationService = new ClusterVisualizationService();
        var clusteringService = new ClusteringService();
        try
        {
            // 1. Load Documents
            var documents =  dataReaderService.ReadDocuments();
            
            //Generate Embeddings
            var embeddings = await embeddingService.GenerateEmbeddings(documents);
            
            clusteringService.ClusterEmbeddings(embeddings);
            
            // 5. Visualization
            Console.WriteLine("Visualization with PCA");
            
            

        } 
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        } 



    }

    static void TestMethod()
    {
        IReadInputData service = new CsvDataReader(AppConstants.DataConstants.InputDataFilePath);
        
        List<Document> documents = service.ReadDocuments();
        Console.WriteLine("Document Count: " + documents.Count);
        
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        EmbeddingGenerationOptions options = new() { Dimensions = 15 };

        List<Document> ll = new List<Document>();
        int cnt = 0;
        foreach (var document in documents)
        {
            OpenAIEmbedding embedding = client.GenerateEmbedding(document.Content, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();
            ++cnt;
            Console.Write($"Document Vector for {cnt} : ");
            Console.WriteLine(string.Join(", ", vector.ToArray().Select(x => x.ToString("F4"))));
            document.Embedding = embedding.ToFloats().ToArray().Select(x => (double)x).ToArray();
            // ll.Add(documents[i]);
            // Console.WriteLine(Regex.Replace(documents[i].Content.Trim(), @"\r\n?|\n", " "));
            // Console.WriteLine("#############################\n#########################\n######################");
            // Console.WriteLine(documents[i].Category);
            // Console.WriteLine("#############################\n#########################\n######################");
        }
        
        string json = JsonSerializer.Serialize(ll, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(json);
        //File.WriteAllText(Environment.GetEnvironmentVariable("JSON_WRITE_PATH") + "embedding.json", json);
        string readJson = File.ReadAllText(Environment.GetEnvironmentVariable("JSON_WRITE_PATH") + "embedding.json");
        List<Document>? documentz = JsonSerializer.Deserialize<List<Document>>(readJson);
        foreach (Document document in documentz)
        {
            Debug.Assert(document.Embedding != null, "document.Embedding != null");
            Console.WriteLine(string.Join(", ", document.Embedding));
            Console.WriteLine(string.IsNullOrEmpty(document.Content));
        }
        
    }
}
