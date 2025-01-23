namespace GPTDocumentClustering.Helper;

public class JsonFileUtil
{
    static bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }
}