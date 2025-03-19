## 1. Project Overview

**Title:** Clustering Documents and Visualization of Embedding Vector Space

**Objective:** To generate embeddings for a set of documents using OpenAI's GPT, cluster the documents based on their embeddings, and visualize the clusters in a reduced-dimensional space. Additionally, calculate and verify the cosine similarity between documents within and across clusters.

## 2. Data Collection and Preparation

The dataset used in this project is a text document classification dataset containing **2,225 text documents** categorized into **five distinct classes**: politics, sport, tech, entertainment, and business. This dataset is well-suited for tasks such as **document classification** and **document clustering**, as it provides a diverse range of textual data across multiple domains.

### Data Collection Process

The dataset was collected from publicly available sources, such as news articles, blogs, and other text-based resources, ensuring a balanced representation of each category. Each document is labeled with its corresponding category, making it suitable for supervised learning tasks.

### Preprocessing Steps

To prepare the dataset for analysis, the following preprocessing steps were applied:

1. **Removing Special Characters**: Punctuation, symbols, and other non-alphanumeric characters were removed to reduce noise in the text data.

2. **Converting to Lowercase**: All text was converted to lowercase to ensure uniformity and avoid duplication of words due to case sensitivity (e.g., "Tech" and "tech" are treated as the same word).

3. **Tokenization**: The text was split into individual words or tokens to facilitate further analysis.

4. **Stopword Removal**: Common stopwords (e.g., "the", "is", "and") were removed to focus on meaningful words that contribute to the classification task.

5. **Stemming/Lemmatization**: Words were reduced to their base or root form (e.g., "running" to "run") to normalize the text and reduce redundancy.


## 3. Generate Embeddings

The OpenAI API was used to generate embeddings for the text documents. The API provides powerful pre-trained models like **text-embedding-ada-002**, which converts text into high-dimensional vector representations. These embeddings capture semantic meaning, making them ideal for document classification and clustering tasks. By sending text data to the API, it returns numerical vectors that can be used as input for machine learning models.


## 4. Clustering

The K-means algorithm is an unsupervised machine learning method utilized for clustering data into groups based on shared characteristics. By minimizing intra-cluster variance, the algorithm iteratively assigns data points to the nearest cluster centroids and recalculates the centroids until it reaches convergence or a predefined number of iterations. In this project, the K-means algorithm was applied to cluster documents according to their respective categories, providing an efficient solution for organizing data.

## 5. Visualization Implementation**  
The visualization module generates 2D scatter plots using **PCA-reduced embeddings** to display document groupings. The `VisualizeDocumentClusters` method accepts precomputed 2D coordinates and toggles between two modes:  
1. **Cluster View**: Colors points by algorithm-generated cluster IDs using a rotating color palette.  
2. **Category View**: Colors points by ground-truth labels using predefined category colors.  

The `AnalyzeAndVisualize` method orchestrates the workflow:  
- Creates an output directory if missing  
- Generates two PNG images (`clusters.png`, `categories.png`)  
- Saves cluster evaluation metrics to a text file  

**Key Implementation Details**:  
- Uses ScottPlot library for rendering

---

## **Project Structure**


---


## **Dependencies**
<!-- 
### **For Dataset**


### **For Embedding:**


### **For visualization and Cosine Similarity:**

## Dependencies
-->

This project relies on the following dependencies:

*   **Accord.NET:**
    *   `Accord.Math`
    *   `Accord.Statistics`
    *   _Install via NuGet: `dotnet add package Accord.Math` and `dotnet add package Accord.Statistics`_

*   **ScottPlot:**
    *   _Install via NuGet: `dotnet add package ScottPlot`_

*   **System.Drawing.Common:**
    *   _(Required for .NET 6+)_
    *   _Install via NuGet: `dotnet add package System.Drawing.Common`_

*   **GPTDocumentClustering.Models.Document:**
    *   This is a custom class that you must define in your project. It should include properties for document text, embedding, cluster ID, and category.

*   **System (Part of .NET):**
    *   `System.IO`
    *   `System.Linq`
    *   `System.Collections.Generic`
    *   _These are core .NET libraries and do not require separate installation._

---

## **Setup Instructions**

### 1. **Prerequisites**

- **Programming Language**: .NET Core
- **Dependencies**: Install required NuGet packages

---

## **Usage Instructions**


---

### Sample Output Files  

This process generates the following outputs:

1.  **Visualizations (PNG Images):**
    *   `clusters.png`:  A 2D scatter plot of documents, colored according to their assigned cluster.
    *   `categories.png`: A 2D scatter plot of documents, colored according to their original category.

2.  **Evaluation Report (TXT File: `cluster_evaluation.txt`):**
    *   A text file containing quantitative metrics for assessing the clustering quality. Key metrics include:
        *   Average Intra-Cluster Similarity
        *   Average Inter-Cluster Similarity
        *   Cluster-to-Category Mapping and Purity
        *   Individual Cluster and Category Similarity Scores
        *   Silhouette Coefficient

---

## **Team Members**

-  Aditya Shidhaye
-  Aditya Ramesh
-  Pradeep Patwa
