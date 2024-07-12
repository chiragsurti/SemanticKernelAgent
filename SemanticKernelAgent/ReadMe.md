# Semantic kernel Agent
### Simple chat completion
### Native plugin - Time Plugins folder (TimeInformationPlugin.cs)
### Native plugin - Light Plugins in folder (LightsPlugin.cs)
### Open api specification plugin  (Azure function app swagger url- SemanticKernelAgent.OpenAPI.Plugins project)
### Planner (function calling) - planner will fetch the current time and according to if time is PM then it will switch on table lamp.
### Persona - User, system(Assistant)

```
dotnet add package Microsoft.SemanticKernel
```

# User secret configuration
## add below entry to secret.json file.
```
{
  "ModelId": "gpt-4",
  "Endpoint": "https://{*}.openai.azure.com/",
  "ApiKey": "",
  "IsOpenApiPlugin": "true",
  "LightsOnlinePluginURL": "http://localhost:7168/api/swagger.json" //Azure function app swagger url- SemanticKernelAgent.OpenAPI.Plugins 
}
```
# TODO: Mixed chat agents conversation

# TODO: Open AI assistant
# TODO: Memory, Embedding

Retrieval-Augmented Generation (RAG) 

It's an innovative architecture which combine the strengths of LLM and specific relevant business domain context knowledge base (Knowldge Grounding).

Embedding - It is important concept - Basically it's numerical representation of text in multidimension space called vector.

Embedding types - 
Each embedding type has unique characteristics and is suited for different tasks:

Word2Vec and GloVe are effective for tasks requiring word-level semantics.
ELMo and InferSent offer richer representations for sentence-level tasks, considering the broader context.
SBERT excels in tasks involving sentence similarity, search, and retrieval, due to its ability to produce highly contextualized embeddings.
Chunking algorithm- 
Chunking is the process of breaking down a large text or document into smaller, manageable pieces or "chunks."

1. Fixed-Length Chunking
Description: Divides the text into chunks of a fixed number of tokens (words or characters).
2. Sentence-Based Chunking
Description: Splits the text at sentence boundaries.
3. Paragraph-Based Chunking
Description: Divides the text based on paragraph boundaries.
4. Overlapping Chunking
Description: Creates chunks with overlapping tokens to retain context between chunks.
5.Semantic Chunking
Description: Uses semantic information to divide the text, such as topic modeling or key phrase extraction.

Choosing the Right Algorithm
For simple tasks: Fixed-length or sentence-based chunking is usually sufficient.
For preserving context: Overlapping chunking or semantic chunking is preferred.
For structured documents: Paragraph-based chunking works well.

RAG Implementation
RAG consists of three phases. 
1) Ingestion of document repositories, and databases, that contain proprietary data.
2) Retrieval of relevant data based on user query 
3) Generation of response to the user

Vector Databases
Some of the leading vector databases 
FAISS (Facebook AI Similarity Search):

Developed by Facebook AI, FAISS is a library for efficient similarity search and clustering of dense vectors. It supports CPU and GPU operations for high performance.
Annoy (Approximate Nearest Neighbors Oh Yeah):

Developed by Spotify, Annoy is a C++ library with Python bindings for approximate nearest neighbor search. It’s designed for fast, memory-efficient searches.
Milvus:

An open-source vector database designed to handle large-scale vector data. It supports various indexing methods and integrates with machine learning frameworks like TensorFlow and PyTorch.
Weaviate:

An open-source knowledge graph that combines a vector database with semantic search capabilities. It allows storing and retrieving data based on vector similarity and context.
Pinecone:

A managed vector database service that provides scalable and high-performance similarity search and embedding operations.
PGVector (Open-Source)
Description: PGVector is a PostgreSQL extension that adds support for vector similarity search.
Elasticsearch
Description: Elasticsearch is a widely used search engine that now includes support for vector search.
Vespa
Description: Vespa is a real-time serving engine for large-scale applications, including vector search.
Azure AI Search
Supports storing and searching high-dimensional vectors, enabling similarity search.
Integrates with various AI and machine learning models to generate vector embeddings.
Allows combining vector search with traditional keyword search for hybrid search capabilities.
Chroma DB
Description: Chroma is a vector database specifically designed for handling and querying high-dimensional vector data. It is particularly suited for use cases involving embeddings generated from machine learning models, such as those in natural language processing, computer vision, and recommendation systems

Large Language Model (LLM)

For Sentence-Level Embeddings: SBERT is often the best choice due to its fine-tuning on sentence similarity tasks.
For Token-Level Embeddings: BERT and RoBERTa are strong candidates, offering context-aware token embeddings.
For Multimodal Applications: CLIP is highly effective when dealing with both text and images.
For General-Purpose Embeddings: GPT-3 and other large models from OpenAI provide versatile embeddings suitable for a wide range of tasks.


Advantages of RAG
Cost-effective implementation
Current information
Enhanced user trust
More developer control