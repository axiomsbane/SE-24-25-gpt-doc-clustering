using GPTDocumentClustering.Models;

namespace GPTDocumentClustering.Services.Embedding;

/// <summary>
/// This service implements the functionality of getting embeddings
/// for all the documents
/// </summary>
public class EmbeddingService
{
    private readonly EmbeddingGenerator _embeddingGenerator = new();

    public async Task<List<Document>>  GenerateEmbeddings(List<Document> documents)
    {
        // Create tasks to generate embeddings for all documents concurrently
        var tasks = documents.Select(async document =>
        {
            document.Embedding = await _embeddingGenerator.GenerateEmbeddings(document.Content);
            Console.WriteLine($"Document Vector for {document.SerialNo} category {document.Category} : ");
            //Console.WriteLine(string.Join(", ", document.Embedding.Select(x => x.ToString("F4"))));
            return document;
        });
        
        // Wait for all tasks to complete and return the processed documents
        return (await Task.WhenAll(tasks)).ToList();
    }
}