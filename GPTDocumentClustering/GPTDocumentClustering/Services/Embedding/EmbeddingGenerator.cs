using GPTDocumentClustering.Helper;
using OpenAI.Embeddings;

namespace GPTDocumentClustering.Services.Embedding;

public class EmbeddingGenerator
{
    private EmbeddingClient _client = new("text-embedding-3-large", AppConstants.OpenAI.ApiKey);
    private EmbeddingGenerationOptions _options = new() { Dimensions = AppConstants.OpenAI.EmbeddingLength };

    public Task<double[]>  GenerateEmbeddings(string text)
    {
        OpenAIEmbedding embedding = _client.GenerateEmbedding(text, _options);
        Console.WriteLine(embedding.ToString());
        return Task.FromResult(embedding.ToFloats().ToArray().Select(x => (double)x).ToArray());
    }
}