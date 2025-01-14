using CsvHelper.Configuration;
using GPTDocumentClustering.Models;

namespace GPTDocumentClustering.Services.InputData;

internal sealed class MyCsvMap : ClassMap<Document>
{
    public MyCsvMap()
    {
        Map(x => x.Content).Name("Text");
        Map(x => x.Category).Name("Label");
    }
}