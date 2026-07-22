using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Abstraction.AI
{
    /// <summary>
    /// Thin abstraction over whatever model actually serves text-generation and embeddings
    /// (by default an OpenAI-API-compatible endpoint hosting MedGemma, e.g. via Ollama, vLLM,
    /// LM Studio or a Hugging Face TGI deployment - see appsettings.json "AI" section).
    /// Keeping this behind an interface means the Application layer (services + MCP tools) never
    /// depends on a specific hosting technology, and the model can be swapped in Infrastructure
    /// without touching business logic.
    /// </summary>
    public interface IMedicalAIClient
    {
        /// <summary>
        /// Sends a chat-style completion request (system + user prompt) and returns the raw text
        /// produced by the model.
        /// </summary>
        Task<string> GenerateAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces an embedding vector for the given text, used both when indexing content into
        /// the RAG vector store and when embedding an incoming chatbot question.
        /// </summary>
        Task<float[]> EmbedAsync(string text, CancellationToken cancellationToken = default);

        /// <summary>Name of the chat/completion model currently configured (used for auditing/logging).</summary>
        string ChatModelName { get; }

        /// <summary>Name of the embedding model currently configured (stored alongside each RAG chunk).</summary>
        string EmbeddingModelName { get; }
    }
}
