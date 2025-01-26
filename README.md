# SE-24-25-gpt-doc-clustering

## ML 24/25-08 Clustering documents and visualization of Embedding Vector Space

### Project Description:
In this project, students must leverage OpenAI's GPT to create embeddings for a set of documents. The solution should utilize the OpenAI NuGet package, as demonstrated in this example: OpenAI Samples.
You can use any set of documents belonging to different semantic categories. For example, these could include orders, invoices, or support requests from various problem domains.
Your task is to generate embeddings for all documents and then use a clustering mechanism, such as the KNN algorithm (refer to the Frankfurt University of Applied Sciences' KNN algorithm), to cluster the documents and ensure they are grouped correctly. Additionally, provide a solution to visualize all scalar values within clusters across the entire vector space.
Additionally, you must also calculate the cosine similarity between documents and approve that documents inside the same cluster have high similarity. In turn, documents from different clusters must have low similarity.
Find also a way to represent clusters visually, by using some dimension reduction algorithm or similar.
