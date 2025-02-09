using System.Text.Json.Serialization;

namespace GPTDocumentClustering.Models;

public class Document
{
    // Primary identifier
    [JsonIgnore]
    public string Content { get; set; }
    public string Category { get; set; }
    
    public int SerialNo { get; set; }

    // Embedding-related properties
    public double[]? Embedding { get; set; }
    
    
    
    [JsonIgnore]
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