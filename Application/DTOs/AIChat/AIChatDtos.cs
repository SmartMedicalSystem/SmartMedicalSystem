using System.Text.Json;

namespace Application.DTOs.AIChat;

public sealed class AIChatRequestDto
{
    public string Message { get; set; } = string.Empty;
    public int? PatientId { get; set; }
    public int? DoctorId { get; set; }
    public string? ConfirmActionId { get; set; }
}

public sealed class AIChatResponseDto
{
    public string Answer { get; set; } = string.Empty;
    public bool RequiresConfirmation { get; set; }
    public ProposedActionDto? ProposedAction { get; set; }
}

public sealed class ProposedActionDto
{
    public string ActionId { get; set; } = string.Empty;
    public string ToolName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public JsonElement Arguments { get; set; }
}

public sealed class AIChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string? Content { get; set; }
    public List<AIChatToolCall> ToolCalls { get; set; } = new();
    public string? ToolCallId { get; set; }
    public string? Name { get; set; }
}

public sealed class AIChatToolCall
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Arguments { get; set; } = "{}";
}