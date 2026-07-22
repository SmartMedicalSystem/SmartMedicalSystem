using Application.DTOs.Rag;
using Application.Services.Abstraction.AI;
using Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.AI
{
    /// <summary>
    /// Reference RAG implementation: embeddings are stored as JSON float[] on PatientRagDocument
    /// (see Domain.Entities.PatientRagDocument) and similarity search is a brute-force in-memory
    /// cosine similarity over the candidate set (all of a patient's documents, or the whole table
    /// when no patient filter is given).
    ///
    /// This is intentionally simple and fully portable across any SQL Server edition/version. It
    /// scales fine for a single patient's lab history (dozens to low hundreds of chunks). If the
    /// unfiltered (cross-patient) search needs to scale to a very large table, either add a
    /// SQL Server 2025+ native VECTOR column with VECTOR_DISTANCE, or move the vector index to a
    /// dedicated vector database (Qdrant, pgvector, Azure AI Search, ...) behind this same interface.
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

            var candidates = patientId.HasValue
                ? await _uow.PatientRagDocuments.GetByPatientAsync(patientId.Value)
                : await _uow.PatientRagDocuments.GetAllActiveDocumentsAsync();

            var candidateList = candidates.ToList();
            if (candidateList.Count == 0)
                return new List<RagSourceDto>();

            var queryEmbedding = await _aiClient.EmbedAsync(query, cancellationToken);

            var scored = candidateList
                .Select(doc => new
                {
                    Doc = doc,
                    Score = CosineSimilarity(queryEmbedding, doc.GetEmbedding())
                })
                .OrderByDescending(x => x.Score)
                .Take(topK <= 0 ? 5 : topK)
                .ToList();

            return scored.Select(x => new RagSourceDto
            {
                DocumentId = x.Doc.Id,
                PatientId = x.Doc.PatientId,
                PatientResultId = x.Doc.PatientResultId,
                SourceType = x.Doc.SourceType,
                Content = x.Doc.Content,
                SimilarityScore = Math.Round(x.Score, 4)
            }).ToList();
        }

        private static double CosineSimilarity(float[] a, float[] b)
        {
            var length = Math.Min(a.Length, b.Length);
            if (length == 0)
                return 0d;

            double dot = 0, normA = 0, normB = 0;
            for (var i = 0; i < length; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }

            if (normA == 0 || normB == 0)
                return 0d;

            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }
    }
}
