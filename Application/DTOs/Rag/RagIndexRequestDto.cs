namespace Application.DTOs.Rag
{
    /// <summary>Used by the manual "index arbitrary text for a patient" endpoint (e.g. doctor notes).</summary>
    public class RagIndexRequestDto
    {
        public int PatientId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
