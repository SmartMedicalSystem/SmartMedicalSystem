using Application.DTOs.Rag;
using Application.Services.Abstraction.AI;
using Domain.IRepository;
using Microsoft.Data.SqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.AI
{
    /// <summary>
    /// RAG implementation backed by SQL Server 2025's native VECTOR type. Embeddings are stored as
    /// SqlVector&lt;float&gt; on PatientRagDocument (see Domain.Entities.PatientRagDocument), and
    /// similarity search is pushed down to the database via EF.Functions.VectorDistance ("cosine")
    /// - see Infrastructure.Repository.PatientRagDocumentRepository - instead of pulling every
    /// candidate row into memory and scoring it in C#. SQL Server does the nearest-neighbor ranking
    /// itself, which scales far better than the old brute-force in-memory approach once the table
    /// grows beyond a single patient's handful of chunks.
    /// </summary>
    public class RagService : IRagService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMedicalAIClient _aiClient;

        public RagService(IUnitOfWork uow, IMedicalAIClient aiClient)
        {
            _uow = uow;
            _aiClient = aiClient;
        }

        public async Task IndexAsync(int patientId, int? patientResultId, string sourceType, string content, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;

            // Remove any previously indexed chunk for this exact PatientResult so re-generating a
            // report doesn't accumulate stale duplicates in the vector store over time.
            if (patientResultId.HasValue)
            {
                var existing = await _uow.PatientRagDocuments.GetByPatientResultAsync(patientResultId.Value);
                var sameSource = existing.Where(d => d.SourceType == sourceType).ToList();
                foreach (var doc in sameSource)
                {
                    await _uow.PatientRagDocuments.DeleteAsync(doc.Id);
                }
            }

            var embedding = await _aiClient.EmbedAsync(content, cancellationToken);

            var entity = new Domain.Entities.PatientRagDocument(
                patientId, patientResultId, sourceType, content, embedding, _aiClient.EmbeddingModelName);

            await _uow.PatientRagDocuments.AddAsync(entity);
        }

        public async Task<List<RagSourceDto>> SearchAsync(string query, int? patientId, int topK, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<RagSourceDto>();

            var effectiveTopK = topK <= 0 ? 5 : topK;

            var queryEmbedding = await _aiClient.EmbedAsync(query, cancellationToken);
            var queryVector = new SqlVector<float>(queryEmbedding);

            var results = patientId.HasValue
                ? await _uow.PatientRagDocuments.SearchByPatientAsync(patientId.Value, queryVector, effectiveTopK, cancellationToken)
                : await _uow.PatientRagDocuments.SearchAllActiveAsync(queryVector, effectiveTopK, cancellationToken);

            return results.Select(r => new RagSourceDto
            {
                DocumentId = r.Document.Id,
                PatientId = r.Document.PatientId,
                PatientResultId = r.Document.PatientResultId,
                SourceType = r.Document.SourceType,
                Content = r.Document.Content,
                // SQL Server's VECTOR_DISTANCE("cosine", ...) returns a *distance* in [0, 2]
                // (0 = identical). Convert back to a similarity score in the same [-1, 1] range
                // the previous in-memory cosine-similarity implementation produced, so callers
                // (RagChatService, the chat-by-patient and chat-by-group/GroupByPatient modes,
                // and the API response shape) don't need to change.
                SimilarityScore = Math.Round(1d - r.Distance, 4)
            }).ToList();
        }
    }
}
