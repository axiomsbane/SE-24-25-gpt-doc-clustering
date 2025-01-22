using GPTDocumentClustering.Models;

namespace GPTDocumentClustering.Services.Embedding;

public class EmbeddingService
{
    private readonly EmbeddingGenerator _embeddingGenerator = new();

    public async Task<List<Document>>  GenerateEmbeddings(List<Document> documents)
    {
        // Create tasks to generate embeddings for all documents concurrently
        var tasks = documents.Select(async document =>
        {
            document.Embedding = await _embeddingGenerator.GenerateEmbeddings(document.Content);
            return document;
        });
        
        // Wait for all tasks to complete and return the processed documents
        return (await Task.WhenAll(tasks)).ToList();
    }
}