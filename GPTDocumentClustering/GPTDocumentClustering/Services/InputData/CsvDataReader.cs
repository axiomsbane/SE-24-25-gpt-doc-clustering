using System.Globalization;
using System.Reflection.Metadata;
using CsvHelper;
using GPTDocumentClustering.Interfaces.InputData;

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
    

    public List<Models.Document> ReadDocuments()
    {
        using (var reader = new StreamReader(_filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<MyCsvMap>();
            var records = csv.GetRecords<Models.Document>();
            return records.ToList();
        }
    }
}