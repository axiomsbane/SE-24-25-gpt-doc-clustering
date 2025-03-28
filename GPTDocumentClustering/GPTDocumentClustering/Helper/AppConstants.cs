namespace GPTDocumentClustering.Helper;

public static class AppConstants
{

    public static class OpenAI
    {
        public static string ApiKey { get; } = LoadEnvKey("OPENAI_API_KEY");
        public static int EmbeddingLength { get; } = 3072;
    }

    public static class DataConstants
    {
        public static string EmbeddingFilePath { get; } = LoadEnvKey("JSON_WRITE_PATH");
        public static string InputDataFilePath { get; } = LoadEnvKey("INPUT_FILE_PATH");
        
        public static Dictionary<string, string> MyDictionary = new Dictionary<string, string>
        {
            { "0", "Politics" },
            { "1", "Sport" },
            { "2", "Technology" },
            { "3", "Entertainment" }
        };
    }

    private static string LoadEnvKey(string envKey)
    {
        return Environment.GetEnvironmentVariable(envKey) ?? string.Empty;
    }
    
    
}