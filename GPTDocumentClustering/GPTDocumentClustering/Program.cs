using System.Text.RegularExpressions;
using GPTDocumentClustering.Interfaces.InputData;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.InputData;
using OpenAI.Embeddings;

namespace GPTDocumentClustering;
using OpenAI.Chat;

class Program
{
    static void Main(string[] args)
    {
        IReadInputData service = new CsvDataReader(Environment.GetEnvironmentVariable("INPUT_FILE_PATH"));
        
        List<Document> documents = service.ReadDocuments();
        Console.WriteLine("Document Count: " + documents.Count);
        foreach (Document document in documents)
        {
            document.Content = Regex.Replace(document.Content.Trim(), @"\r\n?|\n", " ");
        }
        
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        EmbeddingGenerationOptions options = new() { Dimensions = 15 };
        
        
        for (int i = 0; i < 3; i++)
        {
            OpenAIEmbedding embedding = client.GenerateEmbedding(documents[i].Content, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();
            Console.WriteLine(string.Join(", ", vector.ToArray().Select(x => x.ToString("F4"))));
            //Console.WriteLine(Regex.Replace(documents[i].Content.Trim(), @"\r\n?|\n", " "));
            Console.WriteLine("#############################\n#########################\n######################");
            Console.WriteLine(documents[i].Category);
            Console.WriteLine("#############################\n#########################\n######################");
        }

    }
}