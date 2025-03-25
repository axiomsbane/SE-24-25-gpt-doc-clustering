using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using GPTDocumentClustering.Interfaces.InputData;
using GPTDocumentClustering.Models;

namespace GPTDocumentClustering.Services.InputData;

/// <summary>
/// This class does all the reading and parsing of CSV input data
/// in the project
/// </summary>
public class CsvDataReader : IReadInputData
{
    private readonly string _filePath;
    
    public CsvDataReader(string filePath)
    {
        // Validate input file path before processing
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        _filePath = filePath;
    }
    
    /// <summary>
    /// Maps the CSV columns to the parameters in Document
    /// model class and returns a list of all the parsed documents
    /// </summary>
    /// <returns>document list</returns>
    public List<Document> ReadDocuments()
    {
        List<Document> documents;
        using (var reader = new StreamReader(_filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<MyCsvMap>();
            var records = csv.GetRecords<Document>();
            documents = records.ToList();
        }

        int cnt = 0;
        foreach (Document document in documents)
        {
            document.SerialNo = ++cnt;

            // Ensures consistent text format for further processing
            document.Content = Regex.Replace(document.Content.Trim(), @"\r\n?|\n", " ");
        }
        return documents;
    }
}