using Application.DTOs.AIChat;
using System.Text.Json;

namespace Application.Services.Abstraction.AI;

public interface IPatientChatToolsService
{
    IReadOnlyList<AIChatToolDefinition> GetToolDefinitions();
    bool RequiresConfirmation(string toolName);
    string GetToolDescription(string toolName);
    Task<string> ExecuteAsync(string toolName, JsonElement arguments, int? doctorId, CancellationToken cancellationToken = default);
}
