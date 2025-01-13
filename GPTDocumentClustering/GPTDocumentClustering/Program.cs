namespace GPTDocumentClustering;
using OpenAI.Chat;

class Program
{
    static void Main(string[] args)
    {
        ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        
        int numMessages = Convert.ToInt32(Console.ReadLine());
        for (int i = 0; i < numMessages; i++)
        {
            ChatCompletion completion = client.CompleteChat(Console.ReadLine());
            Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
        }
        
    }
}