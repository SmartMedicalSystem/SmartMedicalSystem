using System;
using Domain.Common;

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
    /// The embedding is stored as a JSON-encoded array of floats (<see cref="EmbeddingJson"/>)
    /// rather than a native SQL Server VECTOR column so the schema stays portable across any
    /// supported SQL Server version. Similarity search (cosine similarity) is performed in the
    /// application layer - see Application.Services.RagService. If you are running SQL Server
    /// 2025+ you can migrate this column to the native VECTOR type and push the similarity
    /// search down to the database for better performance at scale.
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

        /// <summary>JSON-serialized float[] embedding vector produced by the embedding model.</summary>
        public string EmbeddingJson { get; private set; } = "[]";

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
            EmbeddingJson = System.Text.Json.JsonSerializer.Serialize(embedding);
            EmbeddingDimensions = embedding.Length;
        }

        public float[] GetEmbedding()
        {
            if (string.IsNullOrWhiteSpace(EmbeddingJson))
                return Array.Empty<float>();

            return System.Text.Json.JsonSerializer.Deserialize<float[]>(EmbeddingJson) ?? Array.Empty<float>();
        }

        public void UpdateContent(string content, float[] embedding, string embeddingModel)
        {
            Content = Guard.NotNullOrWhiteSpace(content, nameof(content), 8000);
            EmbeddingModel = embeddingModel ?? string.Empty;
            SetEmbedding(embedding);
        }
    }
}
