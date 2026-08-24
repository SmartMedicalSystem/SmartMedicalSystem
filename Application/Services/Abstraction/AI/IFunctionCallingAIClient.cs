using Application.DTOs.AIChat;
using System.Text.Json;

namespace Application.Services.Abstraction.AI;

public interface IFunctionCallingAIClient
{
    Task<AIChatCompletionResult> ChatAsync(
        IReadOnlyList<AIChatMessage> messages,
        IReadOnlyList<AIChatToolDefinition> tools,
        CancellationToken cancellationToken = default);
}

public sealed class AIChatCompletionResult
{
    public string? Content { get; init; }
    public List<AIChatToolCall> ToolCalls { get; init; } = new();
}

public sealed class AIChatToolDefinition
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public JsonElement Parameters { get; init; }
}
