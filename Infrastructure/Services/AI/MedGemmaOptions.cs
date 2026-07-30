namespace Infrastructure.Services.AI
{
    /// <summary>
    /// Bound from the "AI" section of appsettings.json. Targets any OpenAI-API-compatible
    /// server (Ollama's OpenAI-compatible endpoints, vLLM, LM Studio, Hugging Face TGI, etc.)
    /// hosting a MedGemma checkpoint, so the recommended model can run fully on-prem/offline -
    /// important for patient data.
    /// </summary>
    public class MedGemmaOptions
    {
        /// <summary>Base URL of the inference server, e.g. "http://localhost:11434" for Ollama.</summary>
        public string BaseUrl { get; set; } = "http://localhost:11434";

        /// <summary>Relative path of the chat-completions endpoint (OpenAI-compatible schema).</summary>
        public string ChatCompletionsPath { get; set; } = "/v1/chat/completions";

        /// <summary>Relative path of the embeddings endpoint (OpenAI-compatible schema).</summary>
        public string EmbeddingsPath { get; set; } = "/v1/embeddings";

        /// <summary>Model name/tag for text generation, e.g. "medgemma-4b-it" or "medgemma-27b-text-it".</summary>
        public string ChatModel { get; set; } = "medgemma-4b-it";

        /// <summary>
        /// Model name/tag for embeddings. MedGemma itself is not an embedding model, so a
        /// dedicated embedding model is used for the RAG vector store (e.g. "nomic-embed-text",
        /// "mxbai-embed-large", or an OpenAI "text-embedding-3-small" if you point BaseUrl at
        /// a hosted provider instead of a local one).
        /// </summary>
        public string EmbeddingModel { get; set; } = "nomic-embed-text";

        /// <summary>Optional bearer token, only needed for hosted providers that require auth.</summary>
        public string? ApiKey { get; set; }

        /// <summary>Sampling temperature for report/summary generation. Kept low for factual, reproducible clinical text.</summary>
        public double Temperature { get; set; } = 0.2;

        /// <summary>Max tokens the model is allowed to generate per call.</summary>
        public int MaxOutputTokens { get; set; } = 1024;

        /// <summary>HTTP timeout in seconds for both chat and embedding calls.</summary>
        public int TimeoutSeconds { get; set; } = 120;
    }
}
