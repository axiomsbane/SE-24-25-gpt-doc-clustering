using GPTDocumentClustering.Interfaces.InputData;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services.InputData;

namespace GPTDocumentClustering;
using OpenAI.Chat;

class Program
{
    static void Main(string[] args)
    {
        // ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        //
        //
        // int numMessages = Convert.ToInt32(Console.ReadLine());
        // for (int i = 0; i < numMessages; i++)
        // {
        //     ChatCompletion completion = client.CompleteChat(Console.ReadLine());
        //     Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
        // }
        IReadInputData service = new CsvDataReader(Environment.GetEnvironmentVariable("INPUT_FILE_PATH"));
        
        List<Document> documents = service.ReadDocuments();
        Console.WriteLine("Document Count: " + documents.Count);
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine(documents[i].Content.Trim());
            Console.WriteLine("#############################\n#########################\n######################");
            Console.WriteLine(documents[i].Category);
            Console.WriteLine("#############################\n#########################\n######################");
        }

    }
}