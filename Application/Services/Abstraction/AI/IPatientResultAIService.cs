using Application.DTOs.AI;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Abstraction.AI
{
    /// <summary>
    /// Generates AI content for a single PatientResult: summarizes every PatientResultElement it
    /// contains against normal ranges, then asks the AI model for a classified report and
    /// suggestions. Backs both the MCP tools (Application/MCPTools) and the
    /// IPatientResultService.GenerateAIAnalysisAsync convenience method exposed over REST.
    /// </summary>
    public interface IPatientResultAIService
    {
        /// <summary>
        /// Builds the deterministic element-vs-range breakdown for a PatientResult without calling
        /// the AI model - useful on its own (e.g. for a lightweight MCP "summarize" tool) and as the
        /// grounding context fed into <see cref="GenerateAnalysisAsync"/>.
        /// </summary>
        Task<PatientResultAIAnalysisDto> SummarizeElementsAsync(int patientResultId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Full pipeline: summarizes the elements, calls the AI model for the classified report and
        /// suggestions, persists the results onto the PatientResult entity, and indexes the
        /// generated text into the RAG vector store for the chatbot.
        /// </summary>
        Task<PatientResultAIAnalysisDto> GenerateAnalysisAsync(int patientResultId, CancellationToken cancellationToken = default);
    }
}
