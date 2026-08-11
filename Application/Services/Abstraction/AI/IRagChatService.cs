using Application.DTOs.Rag;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Abstraction.AI
{
    /// <summary>
    /// Retrieval-Augmented-Generation chatbot: retrieves the most relevant indexed chunks for a
    /// question (optionally scoped to one patient) and asks the AI model to answer using only that
    /// context. This is what the front-end chatbot widget calls.
    /// </summary>
    public interface IRagChatService
    {
        Task<RagChatResponseDto> AskAsync(RagChatRequestDto request, CancellationToken cancellationToken = default);
    }
}
