using OpenAI.Embeddings;

namespace GPTDocumentClustering.Services.Embedding;

public class EmbeddingGenerator
{
    private EmbeddingClient _client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
    private EmbeddingGenerationOptions _options = new() { Dimensions = 15 };

    public Task<float[]>  GenerateEmbeddings(string text)
    {
        OpenAIEmbedding embedding = _client.GenerateEmbedding(text, _options);
        return Task.FromResult(embedding.ToFloats().ToArray());
    }
}