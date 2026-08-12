using Domain.Common;
using Microsoft.Data.SqlTypes;
using System;

namespace Domain.Entities
{
    /// <summary>
    /// Source of the text that was embedded into a <see cref="PatientRagDocument"/>.
    /// Kept as simple string constants (rather than an enum) so new AI-generated
    /// content types can be indexed without a migration to alter a check constraint.
    /// </summary>
    public static class RagSourceTypes
    {
        public const string ResultSummary = "ResultSummary";
        public const string ResultReport = "ResultReport";
        public const string ResultSuggestion = "ResultSuggestion";
        public const string FullPatientReport = "FullPatientReport";
        public const string Manual = "Manual";
    }

    /// <summary>
    /// A single "chunk" of AI-generated (or manually supplied) medical text belonging to a
    /// patient, together with its embedding vector, used as the knowledge base for the
    /// Retrieval-Augmented-Generation (RAG) chatbot exposed to the front-end.
    ///
    /// The embedding is persisted using SQL Server 2025's native VECTOR(1536) column type via
    /// EF Core 10's built-in <see cref="SqlVector{T}"/> support (see
    /// Infrastructure.Context.Configurations.PatientRagDocumentConfiguration). Similarity search
    /// is pushed down to the database with EF.Functions.VectorDistance ("cosine") instead of
    /// being computed in the application layer - see
    /// Infrastructure.Repository.PatientRagDocumentRepository / Application.Services.AI.RagService.
    /// </summary>
    public class PatientRagDocument : BaseEntity
    {
        public int PatientId { get; private set; }

        /// <summary>Optional link back to the PatientResult this chunk was generated from.</summary>
        public int? PatientResultId { get; private set; }

        /// <summary>One of <see cref="RagSourceTypes"/>.</summary>
        public string SourceType { get; private set; } = RagSourceTypes.Manual;

        /// <summary>The raw text that was embedded (also used as the RAG "passage" returned to the LLM).</summary>
        public string Content { get; private set; } = string.Empty;

        /// <summary>
        /// Native SQL Server 2025 VECTOR(1536) column. Always populated - a document can't exist
        /// without an embedding, enforced both here (constructor requires it) and at the database
        /// level (see PatientRagDocumentConfiguration: HasColumnType("vector(1536)").IsRequired()).
        /// </summary>
        public SqlVector<float> EmbeddingVector { get; private set; }

        /// <summary>Number of dimensions in the embedding vector (cached for quick sanity checks).</summary>
        public int EmbeddingDimensions { get; private set; }

        /// <summary>Name of the embedding model used, so mixed-model indexes can be detected/rebuilt.</summary>
        public string EmbeddingModel { get; private set; } = string.Empty;

        // Navigation Properties
        public Patient Patient { get; set; } = null!;
        public PatientResult? PatientResultRef { get; set; }

        private PatientRagDocument() { }

        public PatientRagDocument(int patientId, int? patientResultId, string sourceType, string content,
            float[] embedding, string embeddingModel)
        {
            PatientId = Guard.Positive(patientId, nameof(patientId));
            PatientResultId = patientResultId.HasValue ? Guard.Positive(patientResultId.Value, nameof(patientResultId)) : null;
            SourceType = Guard.NotNullOrWhiteSpace(sourceType, nameof(sourceType), 50);
            Content = Guard.NotNullOrWhiteSpace(content, nameof(content), 8000);
            EmbeddingModel = embeddingModel ?? string.Empty;
            SetEmbedding(embedding);
        }

        public void SetEmbedding(float[] embedding)
        {
            ArgumentNullException.ThrowIfNull(embedding);

            if (embedding.Length == 0)
                throw new ArgumentException("Embedding cannot be empty.", nameof(embedding));

            EmbeddingVector = new SqlVector<float>(embedding);
            EmbeddingDimensions = embedding.Length;
        }

        public float[] GetEmbedding()
        {
            return EmbeddingVector.Memory.ToArray();
        }

        public void UpdateContent(string content, float[] embedding, string embeddingModel)
        {
            Content = Guard.NotNullOrWhiteSpace(content, nameof(content), 8000);
            EmbeddingModel = embeddingModel ?? string.Empty;
            SetEmbedding(embedding);
        }
    }
}
