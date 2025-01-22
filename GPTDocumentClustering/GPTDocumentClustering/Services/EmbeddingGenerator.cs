using OpenAI.Embeddings;

namespace GPTDocumentClustering.Services;

public class EmbeddingGenerator
{
    private EmbeddingClient _client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
    private EmbeddingGenerationOptions _options = new() { Dimensions = 15 };

    public float[] GenerateEmbeddings(string text)
    {
        OpenAIEmbedding embedding = _client.GenerateEmbedding(text, _options);
        return embedding.ToFloats().ToArray();
    }
}