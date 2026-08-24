using Application.DTOs.AIChat;
using Application.Services.Abstraction.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services.AI;

public sealed class OpenRouterFunctionCallingClient : IFunctionCallingAIClient
{
    private readonly HttpClient _httpClient;
    private readonly FunctionCallingOptions _options;
    private readonly ILogger<OpenRouterFunctionCallingClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public OpenRouterFunctionCallingClient(
        HttpClient httpClient,
        IOptions<FunctionCallingOptions> options,
        ILogger<OpenRouterFunctionCallingClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        if (_httpClient.BaseAddress is null && !string.IsNullOrWhiteSpace(_options.BaseUrl))
            _httpClient.BaseAddress = new Uri(EnsureTrailingSlash(_options.BaseUrl));

        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        if (!_httpClient.DefaultRequestHeaders.Contains("HTTP-Referer"))
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://smartmedicalsystem.app");
        if (!_httpClient.DefaultRequestHeaders.Contains("X-Title"))
            _httpClient.DefaultRequestHeaders.Add("X-Title", "SmartMedicalSystem Function Calling");
    }

    public async Task<AIChatCompletionResult> ChatAsync(
        IReadOnlyList<AIChatMessage> messages,
        IReadOnlyList<AIChatToolDefinition> tools,
        CancellationToken cancellationToken = default)
    {
        var request = new OpenRouterChatRequest
        {
            Model = _options.ChatModel,
            Temperature = _options.Temperature,
            MaxTokens = _options.MaxOutputTokens,
            Messages = messages.Select(ToWireMessage).ToList(),
            Tools = tools.Select(t => new OpenRouterTool
            {
                Type = "function",
                Function = new OpenRouterFunction
                {
                    Name = t.Name,
                    Description = t.Description,
                    Parameters = t.Parameters
                }
            }).ToList(),
            ToolChoice = "auto"
        };

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                NormalizePath(_options.ChatCompletionsPath), request, JsonOptions, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OpenRouter function-calling request failed: {StatusCode} {Body}", (int)response.StatusCode, body);
                throw new InvalidOperationException($"Function-calling model request failed with HTTP {(int)response.StatusCode}: {body}");
            }

            var payload = await response.Content.ReadFromJsonAsync<OpenRouterChatResponse>(JsonOptions, cancellationToken)
                ?? throw new InvalidOperationException("OpenRouter returned an empty response.");

            var choice = payload.Choices?.FirstOrDefault()
                ?? throw new InvalidOperationException("OpenRouter returned no choices.");

            return new AIChatCompletionResult
            {
                Content = choice.Message?.Content,
                ToolCalls = choice.Message?.ToolCalls?.Select(tc => new AIChatToolCall
                {
                    Id = tc.Id ?? Guid.NewGuid().ToString("N"),
                    Name = tc.Function?.Name ?? string.Empty,
                    Arguments = tc.Function?.Arguments ?? "{}"
                }).Where(tc => !string.IsNullOrWhiteSpace(tc.Name)).ToList() ?? new()
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not InvalidOperationException)
        {
            _logger.LogError(ex, "Unable to reach OpenRouter function-calling endpoint.");
            throw new InvalidOperationException("Unable to reach the OpenRouter function-calling model. Verify FunctionCallingAI settings and network connectivity.", ex);
        }
    }

    private static OpenRouterMessage ToWireMessage(AIChatMessage message) => new()
    {
        Role = message.Role,
        Content = message.Content,
        Name = message.Name,
        ToolCallId = message.ToolCallId,
        ToolCalls = message.ToolCalls.Count == 0 ? null : message.ToolCalls.Select(tc => new OpenRouterToolCall
        {
            Id = tc.Id,
            Type = "function",
            Function = new OpenRouterFunctionCall { Name = tc.Name, Arguments = tc.Arguments }
        }).ToList()
    };

    private static string EnsureTrailingSlash(string url) => url.EndsWith("/", StringComparison.Ordinal) ? url : url + "/";
    private static string NormalizePath(string path) => path.TrimStart('/');

    private sealed class OpenRouterChatRequest
    {
        [JsonPropertyName("model")] public string Model { get; set; } = string.Empty;
        [JsonPropertyName("messages")] public List<OpenRouterMessage> Messages { get; set; } = new();
        [JsonPropertyName("temperature")] public double Temperature { get; set; }
        [JsonPropertyName("max_tokens")] public int MaxTokens { get; set; }
        [JsonPropertyName("tools")] public List<OpenRouterTool> Tools { get; set; } = new();
        [JsonPropertyName("tool_choice")] public string ToolChoice { get; set; } = "auto";
    }

    private sealed class OpenRouterMessage
    {
        [JsonPropertyName("role")] public string Role { get; set; } = string.Empty;
        [JsonPropertyName("content")] public string? Content { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("tool_call_id")] public string? ToolCallId { get; set; }
        [JsonPropertyName("tool_calls")] public List<OpenRouterToolCall>? ToolCalls { get; set; }
    }

    private sealed class OpenRouterTool
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "function";
        [JsonPropertyName("function")] public OpenRouterFunction Function { get; set; } = new();
    }

    private sealed class OpenRouterFunction
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
        [JsonPropertyName("parameters")] public JsonElement Parameters { get; set; }
    }

    private sealed class OpenRouterToolCall
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; } = "function";
        [JsonPropertyName("function")] public OpenRouterFunctionCall? Function { get; set; }
    }

    private sealed class OpenRouterFunctionCall
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("arguments")] public string Arguments { get; set; } = "{}";
    }

    private sealed class OpenRouterChatResponse
    {
        [JsonPropertyName("choices")] public List<OpenRouterChoice>? Choices { get; set; }
    }

    private sealed class OpenRouterChoice
    {
        [JsonPropertyName("message")] public OpenRouterMessage? Message { get; set; }
    }
}
