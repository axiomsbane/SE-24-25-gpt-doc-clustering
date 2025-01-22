namespace GPTDocumentClustering.Models;

public class Document
{
    // Primary identifier
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } 
    public string Category { get; set; }

    // Embedding-related properties
    public float[]? Embedding { get; set; }
    public int? ClusterId { get; set; }

    public Document(string content, string category)
    {
        Content = content;
        Category = category;
    }
    
    /// <remarks>
    /// Empty constructor is kept as CsvHelper needs this
    /// for its functioning
    /// </remarks>
    public Document() {}
}