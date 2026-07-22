namespace Application.DTOs.Rag
{
    public class RagChatRequestDto
    {
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// Optional: scope retrieval to a single patient's indexed documents (recommended for the
        /// doctor-facing chatbot). Leave null to search across every patient the caller can access.
        /// </summary>
        public int? PatientId { get; set; }

        /// <summary>How many top-matching chunks to retrieve as context. Default 5.</summary>
        public int TopK { get; set; } = 5;
    }
}
