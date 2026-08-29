using Application.DTOs.AIChat;
using Application.Services.Abstraction.AI;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Services.AI;

public sealed class FunctionCallingChatService : IFunctionCallingChatService
{
    private readonly IFunctionCallingAIClient _client;
    private readonly IPatientChatToolsService _tools;
    private readonly ILogger<FunctionCallingChatService> _logger;
    private readonly IPendingActionStore _pendingActions;

    private const int MaxToolRounds = 5;

    public FunctionCallingChatService(
        IFunctionCallingAIClient client,
        IPatientChatToolsService tools,
        ILogger<FunctionCallingChatService> logger,
        IPendingActionStore pendingActionStore)
    {
        _client = client;
        _tools = tools;
        _logger = logger;
        _pendingActions = pendingActionStore;
    }

    public async Task<AIChatResponseDto> ChatAsync(
        AIChatRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Message is required.");

        // Confirmation request
        if (!string.IsNullOrWhiteSpace(request.ConfirmActionId))
        {
            return await ConfirmAsync(
                request.ConfirmActionId!,
                cancellationToken);
        }

        var messages = new List<AIChatMessage>
        {
            new()
            {
                Role = "system",
                Content = BuildSystemPrompt(request.PatientId)
            },
            new()
            {
                Role = "user",
                Content = request.Message.Trim()
            }
        };

        for (var round = 0; round < MaxToolRounds; round++)
        {
            var completion = await _client.ChatAsync(
                messages,
                _tools.GetToolDefinitions(),
                cancellationToken);

            // No tool call => final AI response
            if (completion.ToolCalls.Count == 0)
            {
                return new AIChatResponseDto
                {
                    Answer = completion.Content?.Trim() ?? string.Empty,
                    RequiresConfirmation = false,
                    ProposedAction = null
                };
            }

            // Add assistant tool-call message
            messages.Add(new AIChatMessage
            {
                Role = "assistant",
                Content = completion.Content,
                ToolCalls = completion.ToolCalls
            });

            foreach (var call in completion.ToolCalls)
            {
                // ---------------------------------------------------------
                // Actions that modify/persist medical data require
                // confirmation before execution.
                // ---------------------------------------------------------
                if (_tools.RequiresConfirmation(call.Name))
                {
                    var actionId = Guid.NewGuid().ToString("N");

                    var args = ParseArguments(call.Arguments);

                    var pendingAction = new PendingAction(
                        call.Name,
                        args,
                        request.DoctorId,
                        DateTimeOffset.UtcNow.AddMinutes(10));

                    _pendingActions.Add(
                        actionId,
                        pendingAction);

                    _logger.LogInformation(
                        "Pending AI action created. ActionId: {ActionId}, Tool: {ToolName}, DoctorId: {DoctorId}",
                        actionId,
                        call.Name,
                        request.DoctorId);

                    return new AIChatResponseDto
                    {
                        RequiresConfirmation = true,

                        ProposedAction = new ProposedActionDto
                        {
                            ActionId = actionId,
                            ToolName = call.Name,
                            Description = _tools.GetToolDescription(call.Name),
                            Arguments = args
                        },

                        Answer =
                            "I can do that, but this action will change stored medical-system data. " +
                            "Please confirm before I continue."
                    };
                }

                // ---------------------------------------------------------
                // Read-only tools can execute immediately.
                // ---------------------------------------------------------
                try
                {
                    var args = ParseArguments(call.Arguments);

                    var toolResult = await _tools.ExecuteAsync(
                        call.Name,
                        args,
                        request.DoctorId,
                        cancellationToken);

                    messages.Add(new AIChatMessage
                    {
                        Role = "tool",
                        ToolCallId = call.Id,
                        Name = call.Name,
                        Content = toolResult
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "AI tool {ToolName} failed.",
                        call.Name);

                    messages.Add(new AIChatMessage
                    {
                        Role = "tool",
                        ToolCallId = call.Id,
                        Name = call.Name,
                        Content = JsonSerializer.Serialize(
                            new
                            {
                                error = ex.Message
                            })
                    });
                }
            }
        }

        return new AIChatResponseDto
        {
            Answer =
                "I could not complete the requested tool workflow within the allowed number of steps.",
            RequiresConfirmation = false,
            ProposedAction = null
        };
    }

    private async Task<AIChatResponseDto> ConfirmAsync(
        string actionId,
        CancellationToken cancellationToken)
    {
        // -------------------------------------------------------------
        // IMPORTANT:
        // TryRemove is intentional.
        //
        // Once the user confirms an action, remove it immediately so
        // the same action cannot be executed twice.
        // -------------------------------------------------------------
        if (!_pendingActions.TryRemove(
                actionId,
                out var storedAction))
        {
            throw new InvalidOperationException(
                "The requested confirmation has expired or is no longer available. Please ask again.");
        }

        if (storedAction is not PendingAction action)
        {
            _logger.LogError(
                "Invalid pending action object for ActionId {ActionId}.",
                actionId);

            throw new InvalidOperationException(
                "The requested confirmation is invalid. Please ask again.");
        }

        // Check expiration AFTER retrieving the action.
        if (action.ExpiresAt < DateTimeOffset.UtcNow)
        {
            _logger.LogInformation(
                "Pending AI action expired. ActionId: {ActionId}, Tool: {ToolName}",
                actionId,
                action.ToolName);

            throw new InvalidOperationException(
                "The requested confirmation has expired. Please ask again.");
        }

        _logger.LogInformation(
            "Executing confirmed AI action. ActionId: {ActionId}, Tool: {ToolName}, DoctorId: {DoctorId}",
            actionId,
            action.ToolName,
            action.DoctorId);

        var result = await _tools.ExecuteAsync(
            action.ToolName,
            action.Arguments,
            action.DoctorId,
            cancellationToken);

        return new AIChatResponseDto
        {
            Answer = result,
            RequiresConfirmation = false,
            ProposedAction = null
        };
    }

    private static JsonElement ParseArguments(string arguments)
    {
        using var doc = JsonDocument.Parse(
            string.IsNullOrWhiteSpace(arguments)
                ? "{}"
                : arguments);

        return doc.RootElement.Clone();
    }

    private static string BuildSystemPrompt(int? patientId)
    {
        return
            $"You are the SmartMedicalSystem physician assistant. " +
            $"Be concise and factual. " +

            $"Use tools whenever the answer depends on application data; " +
            $"never invent patient data. " +

            $"The current patient context id is " +
            $"{(patientId?.ToString() ?? "not specified")}. " +

            $"Never expose national IDs or internal database details. " +

            $"For actions that modify or persist data, " +
            $"call the corresponding tool but wait for the application " +
            $"confirmation flow; do not claim the action was executed " +
            $"before confirmation. " +

            $"For clinical decisions, clearly state that the output " +
            $"is AI-generated and should be verified against the chart. " +

            $"Prefer the existing RAG tool for questions about " +
            $"longitudinal lab history.";
    }

    // This object is stored inside IPendingActionStore.
    // The store itself is responsible for keeping it alive between
    // different HTTP requests.
    private sealed record PendingAction(
        string ToolName,
        JsonElement Arguments,
        int? DoctorId,
        DateTimeOffset ExpiresAt);
}