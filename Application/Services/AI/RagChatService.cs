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
            // If the caller requested grouping by patient and didn't specify a single patient,
            // fetch a larger candidate set and label passages by patient so the model can
            // reason across individual patients instead of treating all passages as a single
            // patient's history.
            if (request.PatientId == null && request.GroupByPatient)
            {
                // Increase topK to gather more evidence across patients while still bounding work.
                var expandedTopK = Math.Max(request.TopK * 10, 50);
                var sources = await _ragService.SearchAsync(request.Question, null, expandedTopK, cancellationToken);

                var systemPrompt = PatientResultPromptBuilder.BuildChatSystemPromptGroupedByPatient(sources);

                var answer = await _aiClient.GenerateAsync(systemPrompt, request.Question, cancellationToken);

                // Sanitize returned sources to avoid leaking patient identifiers in the API response.
                var sanitized = sources.Select(s => new Application.DTOs.Rag.RagSourceDto
                {
                    DocumentId = s.DocumentId,
                    PatientId = 0,
                    PatientResultId = s.PatientResultId,
                    SourceType = s.SourceType,
                    Content = s.Content,
                    SimilarityScore = s.SimilarityScore
                }).ToList();

                return new RagChatResponseDto
                {
                    Answer = answer,
                    Sources = sanitized
                };
            }

            var defaultSources = await _ragService.SearchAsync(request.Question, request.PatientId, request.TopK, cancellationToken);

            var defaultSystemPrompt = PatientResultPromptBuilder.BuildChatSystemPrompt(
                defaultSources.Select(s => s.Content).ToList());

            var defaultAnswer = await _aiClient.GenerateAsync(defaultSystemPrompt, request.Question, cancellationToken);

            return new RagChatResponseDto
            {
                Answer = defaultAnswer,
                Sources = defaultSources
            };
        }
    }
}
