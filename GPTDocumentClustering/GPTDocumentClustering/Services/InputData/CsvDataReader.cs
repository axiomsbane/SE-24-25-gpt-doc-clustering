using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using GPTDocumentClustering.Interfaces.InputData;
using GPTDocumentClustering.Models;

namespace GPTDocumentClustering.Services.InputData;

public class CsvDataReader : IReadInputData
{
    private readonly string _filePath;
    
    public CsvDataReader(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        _filePath = filePath;
    }
    

    public List<Document> ReadDocuments()
    {
        List<Document> documents = new();
        using (var reader = new StreamReader(_filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<MyCsvMap>();
            var records = csv.GetRecords<Models.Document>();
            documents = records.ToList();
        }

        int cnt = 0;
        foreach (Document document in documents)
        {
            document.SerialNo = ++cnt;
            document.Content = Regex.Replace(document.Content.Trim(), @"\r\n?|\n", " ");
        }
        return documents;
    }
}