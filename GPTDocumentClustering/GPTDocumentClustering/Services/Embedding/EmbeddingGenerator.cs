using GPTDocumentClustering.Helper;
using OpenAI.Embeddings;

namespace GPTDocumentClustering.Services.Embedding;
/// <summary>
/// This class implements the simple functionality of communicating with OpenAPI
/// and returning the embedding vector for the provided string text
/// </summary>
public class EmbeddingGenerator
{
    private EmbeddingClient _client = new("text-embedding-3-large", AppConstants.OpenAI.ApiKey);
    private EmbeddingGenerationOptions _options = new() { Dimensions = AppConstants.OpenAI.EmbeddingLength };
    
    /// <summary>
    /// Returns embedding array of type double[]
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Task<double[]>  GenerateEmbeddings(string text)
    {
        OpenAIEmbedding embedding = _client.GenerateEmbedding(text, _options);
        Console.WriteLine(embedding.ToString());
        return Task.FromResult(embedding.ToFloats().ToArray().Select(x => (double)x).ToArray());
    }
}