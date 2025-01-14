//namespace GPTDocumentClustering;
//using OpenAI.Chat;

//class Program
//{
//    static void Main(string[] args)
//    {
//        ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

//        Console.WriteLine("Please type the number of chat messages you want: ")
//        int numMessages = Convert.ToInt32(Console.ReadLine());
//        for (int i = 0; i < numMessages; i++)
//        {
//            ChatCompletion completion = client.CompleteChat(Console.ReadLine());
//            Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
//        }

//    }
//}
namespace GPTDocumentClustering
{
    using OpenAI.Chat;
    using System;

    public class Program
    {
        private readonly ChatClient _client;
        private readonly TextWriter _output;
        private readonly TextReader _input;

        // Constructor to allow dependency injection for easier testing.
        public Program(ChatClient client, TextReader input = null, TextWriter output = null)
        {
            _client = client;
            _input = input ?? Console.In;
            _output = output ?? Console.Out;
        }

        // Run method to handle user input/output and interact with the ChatClient
        public void Run()
        {
            _output.WriteLine("Please type the number of chat messages you want: ");
            int numMessages;
            if (!int.TryParse(_input.ReadLine(), out numMessages))
            {
                _output.WriteLine("Invalid input.");
                return;
            }

            for (int i = 0; i < numMessages; i++)
            {
                _output.WriteLine("Please type your message: ");
                var userInput = _input.ReadLine();
                ChatCompletion completion = _client.CompleteChat(userInput);
                _output.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
            }
        }

        // Main method to start the program (used for normal execution, but not for testing).
        public static void Main(string[] args)
        {
            var client = new ChatClient(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            var program = new Program(client);
            program.Run();
        }
    }
}
