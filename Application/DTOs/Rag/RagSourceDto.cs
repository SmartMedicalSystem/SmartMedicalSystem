namespace Application.DTOs.Rag
{
    public class RagSourceDto
    {
        public int DocumentId { get; set; }
        public int PatientId { get; set; }
        public int? PatientResultId { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public double SimilarityScore { get; set; }
    }
}
