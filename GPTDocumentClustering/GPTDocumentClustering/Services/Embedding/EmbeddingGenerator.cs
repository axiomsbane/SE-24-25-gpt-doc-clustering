using GPTDocumentClustering.Helper;
using OpenAI.Embeddings;

namespace GPTDocumentClustering.Services.Embedding;

public class EmbeddingGenerator
{
    private EmbeddingClient _client = new("text-embedding-3-small", AppConstants.OpenAI.ApiKey);
    private EmbeddingGenerationOptions _options = new() { Dimensions = AppConstants.OpenAI.EmbeddingLength };

    public Task<float[]>  GenerateEmbeddings(string text)
    {
        OpenAIEmbedding embedding = _client.GenerateEmbedding(text, _options);
        return Task.FromResult(embedding.ToFloats().ToArray());
    }
}