using Application.DTOs.Rag;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Abstraction.AI
{
    /// <summary>
    /// Embeds text and stores/searches it in the PatientRagDocument vector table, used both by the
    /// AI report pipeline (to index newly generated summaries/reports) and by the RAG chatbot.
    /// </summary>
    public interface IRagService
    {
        /// <summary>
        /// Embeds <paramref name="content"/> and stores it as a new (or replacement) chunk for the
        /// given patient/source. If a PatientResultId is supplied, any previously indexed chunks for
        /// that same PatientResult+sourceType are removed first so regenerating a report doesn't
        /// leave stale duplicates in the vector store.
        /// </summary>
        Task IndexAsync(int patientId, int? patientResultId, string sourceType, string content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Embeds <paramref name="query"/> and returns the topK most similar indexed chunks
        /// (cosine similarity), optionally restricted to one patient's documents.
        /// </summary>
        Task<List<RagSourceDto>> SearchAsync(string query, int? patientId, int topK, CancellationToken cancellationToken = default);
    }
}
