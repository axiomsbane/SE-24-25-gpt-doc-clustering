using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using GPTDocumentClustering.Helper;
using GPTDocumentClustering.Interfaces.InputData;
using GPTDocumentClustering.Models;
using GPTDocumentClustering.Services;
using GPTDocumentClustering.Services.Clustering;
using GPTDocumentClustering.Services.Embedding;
using GPTDocumentClustering.Services.InputData;
using GPTDocumentClustering.Services.Validation;
using GPTDocumentClustering.Services.Visualization;
using OpenAI.Embeddings;

namespace GPTDocumentClustering;

class Program
{
    static async Task Main(string[] args)
    {
        // TestMethod();
        // return;

        // Initialize services for different stages of document processing
        // 1. Data Reader: Reads documents from a CSV file
        var dataReaderService = new CsvDataReader(Environment.GetEnvironmentVariable("INPUT_FILE_PATH"));

        // 2. Embedding Service: Converts text documents to numerical vectors
        var embeddingService = new EmbeddingService();

        
        Console.WriteLine("Please choose the embedding size that you want to use: \n");
        Console.WriteLine("The options are: \n 1 -> 512 \n 2 -> 1024 \n 3 -> 2048 \n 4 -> 3072 \n");
        Console.WriteLine("Please type 1, 2, 3 or 4 and press enter : \n ");
        var key = int.Parse(Console.ReadLine() ?? "3072");
        Console.WriteLine($"\n You have chosen size {AppConstants.DataConstants.EmbeddingDict[key]}");

        AppConstants.OpenAI.EmbeddingLength = AppConstants.DataConstants.EmbeddingDict[key];
        Console.WriteLine($"Embedding length set is {AppConstants.OpenAI.EmbeddingLength}");

        // 3. Clustering Service: Groups similar documents together
        var clusteringService = new ClusteringService();
        try
        {
            // Step 1: Load Documents from the specified data source
            var documents =  dataReaderService.ReadDocuments();


            // Step 2: Generate Embeddings for each document
            // Converts text to numerical vectors that represent semantic meaning
            var embeddings = await embeddingService.GenerateEmbeddings(documents);


            // Step 3: Perform Clustering on the generated embeddings
            // Groups documents with similar semantic content
            clusteringService.ClusterEmbeddings(embeddings);


            // Step 4: Visualize Clusters using PCA (Principal Component Analysis)
            // Reduces high-dimensional embedding data to 2D/3D for visualization
            Console.WriteLine("Visualization with PCA");
            ClusterVisualizationService clusterVisualizer = new ClusterVisualizationService(embeddings);
            clusterVisualizer.AnalyzeAndVisualize(Directory.GetCurrentDirectory()+"/Output");


            // Step 5: Validate Clusters using Cosine Similarity
            // Measures the similarity between documents within and across clusters
            CosineSimilarityService cosineSimilarityService = new CosineSimilarityService(embeddings);
            cosineSimilarityService.Analyze(Directory.GetCurrentDirectory()+"/Output");

        } 
        catch (Exception ex)
        {
            // Catch and log any errors that occur during the processing
            Console.WriteLine($"An error occurred: {ex.Message}");
        } 



    }

    static void TestMethod()
    {
        // 1. Read input data using CSV data reader
        IReadInputData service = new CsvDataReader(AppConstants.DataConstants.InputDataFilePath);

        // Load documents from the data source
        List<Document> documents = service.ReadDocuments();
        Console.WriteLine("Document Count: " + documents.Count);


        // 2. Initialize OpenAI Embedding Client
        // Using text-embedding-3-small model with 15 dimensions
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        EmbeddingGenerationOptions options = new() { Dimensions = 15 };


        // 3. Generate and process embeddings for each document
        List<Document> ll = new List<Document>();
        int cnt = 0;
        foreach (var document in documents)
        {
            // Generate embedding for the document
            OpenAIEmbedding embedding = client.GenerateEmbedding(document.Content, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();

            // Increment and display document counter
            ++cnt;
            Console.Write($"Document Vector for {cnt} : ");
            Console.WriteLine(string.Join(", ", vector.ToArray().Select(x => x.ToString("F4"))));

            // Convert embedding to double array and attach to document
            document.Embedding = embedding.ToFloats().ToArray().Select(x => (double)x).ToArray();


            // ll.Add(documents[i]);
            // Console.WriteLine(Regex.Replace(documents[i].Content.Trim(), @"\r\n?|\n", " "));
            // Console.WriteLine("#############################\n#########################\n######################");
            // Console.WriteLine(documents[i].Category);
            // Console.WriteLine("#############################\n#########################\n######################");
        }
        
        string json = JsonSerializer.Serialize(ll, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(json);
        //File.WriteAllText(Environment.GetEnvironmentVariable("JSON_WRITE_PATH") + "embedding.json", json);
        string readJson = File.ReadAllText(Environment.GetEnvironmentVariable("JSON_WRITE_PATH") + "embedding.json");
        List<Document>? documentz = JsonSerializer.Deserialize<List<Document>>(readJson);
        foreach (Document document in documentz)
        {
            Debug.Assert(document.Embedding != null, "document.Embedding != null");
            Console.WriteLine(string.Join(", ", document.Embedding));
            Console.WriteLine(string.IsNullOrEmpty(document.Content));
        }
        
    }
}
