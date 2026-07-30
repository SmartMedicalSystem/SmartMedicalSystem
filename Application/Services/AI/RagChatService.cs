using Application.DTOs.Rag;
using Application.Services.Abstraction.AI;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.AI
{
    public class RagChatService : IRagChatService
    {
        private readonly IRagService _ragService;
        private readonly IMedicalAIClient _aiClient;

        public RagChatService(IRagService ragService, IMedicalAIClient aiClient)
        {
            _ragService = ragService;
            _aiClient = aiClient;
        }

        public async Task<RagChatResponseDto> AskAsync(RagChatRequestDto request, CancellationToken cancellationToken = default)
        {
            var sources = await _ragService.SearchAsync(request.Question, request.PatientId, request.TopK, cancellationToken);

            var systemPrompt = PatientResultPromptBuilder.BuildChatSystemPrompt(
                sources.Select(s => s.Content).ToList());

            var answer = await _aiClient.GenerateAsync(systemPrompt, request.Question, cancellationToken);

            return new RagChatResponseDto
            {
                Answer = answer,
                Sources = sources
            };
        }
    }
}
