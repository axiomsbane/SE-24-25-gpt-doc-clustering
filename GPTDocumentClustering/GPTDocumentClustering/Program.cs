namespace GPTDocumentClustering;

class Program
{
    static void Main(string[] args)
    {
        string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        Console.WriteLine(key);
    }
}