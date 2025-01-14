namespace GPTDocumentClustering.Models;

public class Document(string content, string category)
{
    // Primary identifier
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = content;
    public string Category { get; set; } = category;

    // Embedding-related properties
    public double[]? Embedding { get; set; }
    public int? ClusterId { get; set; }
}