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

To generate embeddings for the text documents, the OpenAI API can be utilized. The API provides powerful pre-trained models like **text-embedding-ada-002**, which converts text into high-dimensional vector representations. These embeddings capture semantic meaning, making them ideal for tasks like document classification and clustering. By sending text data to the API, it returns numerical vectors that can be used as input for machine learning models.


## 4. Clustering

