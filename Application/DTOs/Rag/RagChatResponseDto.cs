using System.Collections.Generic;

namespace Application.DTOs.Rag
{
    public class RagChatResponseDto
    {
        public string Answer { get; set; } = string.Empty;
        public List<RagSourceDto> Sources { get; set; } = new();
        public string Disclaimer { get; set; } =
            "AI-generated answer based on this patient's indexed lab history. Verify against the primary chart before acting on it.";
    }
}
