using CsvHelper.Configuration;
using GPTDocumentClustering.Models;

namespace GPTDocumentClustering.Services.InputData;

/// <summary>
/// This class is needed for defining the mapping
/// from the CSV column name to the parameter in Document class
/// </summary>
internal sealed class MyCsvMap : ClassMap<Document>
{
    public MyCsvMap()
    {
        Map(x => x.Content).Name("Text");
        Map(x => x.Category).Name("Label");
    }
}