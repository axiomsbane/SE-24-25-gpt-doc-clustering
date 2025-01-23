using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using GPTDocumentClustering.Helper;
using GPTDocumentClustering.Interfaces.InputData;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.Embedding;
using GPTDocumentClustering.Services.InputData;
using OpenAI.Embeddings;

namespace GPTDocumentClustering;

class Program
{
    static async Task Main(string[] args)
    {
        
        //TestMethod();
        var dataReaderService = new CsvDataReader(Environment.GetEnvironmentVariable("INPUT_FILE_PATH"));
        var embeddingService = new EmbeddingService();

        try
        {
            // 1. Load Documents
            var documents =  dataReaderService.ReadDocuments();
            
            //Generate Embeddings
            var embeddings = await embeddingService.GenerateEmbeddings(documents);
            

        } catch (Exception ex)
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
        for (int i = 0; i < 3; i++)
        {
            OpenAIEmbedding embedding = client.GenerateEmbedding(documents[i].Content, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();
            Console.WriteLine(string.Join(", ", vector.ToArray().Select(x => x.ToString("F4"))));
            documents[i].Embedding = embedding.ToFloats().ToArray();
            ll.Add(documents[i]);
            Console.WriteLine(Regex.Replace(documents[i].Content.Trim(), @"\r\n?|\n", " "));
            Console.WriteLine("#############################\n#########################\n######################");
            Console.WriteLine(documents[i].Category);
            Console.WriteLine("#############################\n#########################\n######################");
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