using Application.Services.Abstraction.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services.AI
{
    /// <summary>
    /// Calls an OpenAI-Chat-Completions-compatible server (Ollama, vLLM, LM Studio, TGI, ...)
    /// hosting a MedGemma checkpoint for text generation, and a (possibly different) embedding
    /// model for RAG. Both concerns share one HttpClient/BaseUrl because most self-hosted stacks
    /// expose both endpoints on the same host; point EmbeddingsPath at a different server via a
    /// second registration if you split them.
    /// </summary>
    public class MedGemmaAIClient : IMedicalAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly MedGemmaOptions _options;
        private readonly ILogger<MedGemmaAIClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public MedGemmaAIClient(HttpClient httpClient, IOptions<MedGemmaOptions> options, ILogger<MedGemmaAIClient> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;

            // NOTE: BaseAddress must end with '/' and relative paths must NOT start with '/'.
            // Otherwise System.Uri's RFC 3986 §5.3 "absolute path" merge rule silently
            // DISCARDS any path segment already present on BaseAddress (e.g. "/api/v1"),
            // which breaks hosted providers like OpenRouter that live under a sub-path
            // (it happened to work with Ollama only because "http://localhost:11434" has no path).
            if (_httpClient.BaseAddress is null && !string.IsNullOrWhiteSpace(_options.BaseUrl))
                _httpClient.BaseAddress = new Uri(EnsureTrailingSlash(_options.BaseUrl));

            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

            if (!string.IsNullOrWhiteSpace(_options.ApiKey))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            // Optional but recommended by OpenRouter for analytics/rate-limit attribution.
            // Harmless no-ops for Ollama/vLLM/other OpenAI-compatible servers.
            if (!_httpClient.DefaultRequestHeaders.Contains("HTTP-Referer"))
                _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://smartmedicalsystem.app");
            if (!_httpClient.DefaultRequestHeaders.Contains("X-Title"))
                _httpClient.DefaultRequestHeaders.Add("X-Title", "SmartMedicalSystem");
        }

        private static string EnsureTrailingSlash(string url) =>
            url.EndsWith("/", StringComparison.Ordinal) ? url : url + "/";

        private static string NormalizeRelativePath(string path) =>
            path.TrimStart('/');

        public string ChatModelName => _options.ChatModel;
        public string EmbeddingModelName => _options.EmbeddingModel;

        public async Task<string> GenerateAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
        {
            var request = new ChatCompletionRequest
            {
                Model = _options.ChatModel,
                Temperature = _options.Temperature,
                MaxTokens = _options.MaxOutputTokens,
                Messages = new List<ChatMessage>
                {
                    new() { Role = "system", Content = systemPrompt },
                    new() { Role = "user", Content = userPrompt }
                }
            };

            try
            {
                using var response = await _httpClient.PostAsJsonAsync(NormalizeRelativePath(_options.ChatCompletionsPath), request, JsonOptions, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError(
                        "MedGemma chat-completion call failed against {BaseUrl}{Path} with status {StatusCode}: {Body}",
                        _options.BaseUrl, _options.ChatCompletionsPath, (int)response.StatusCode, errorBody);
                    throw new InvalidOperationException(
                        $"AI model call to '{_httpClient.BaseAddress}{NormalizeRelativePath(_options.ChatCompletionsPath)}' " +
                        $"failed with HTTP {(int)response.StatusCode} {response.StatusCode}: {errorBody}");
                }

                var payload = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(JsonOptions, cancellationToken);
                var content = payload?.Choices?.Count > 0 ? payload.Choices[0].Message?.Content : null;

                if (string.IsNullOrWhiteSpace(content))
                    throw new InvalidOperationException("The AI model returned an empty response.");

                return content.Trim();
            }
            catch (Exception ex) when (ex is not OperationCanceledException and not InvalidOperationException)
            {
                _logger.LogError(ex, "MedGemma chat-completion call failed against {BaseUrl}{Path}", _options.BaseUrl, _options.ChatCompletionsPath);
                throw new InvalidOperationException(
                    $"Unable to reach the AI model at '{_options.BaseUrl}{_options.ChatCompletionsPath}'. " +
                    $"Verify the AI:BaseUrl setting and network connectivity. Underlying error: {ex.Message}", ex);
            }
        }

        public async Task<float[]> EmbedAsync(string text, CancellationToken cancellationToken = default)
        {
            var request = new EmbeddingRequest
            {
                Model = _options.EmbeddingModel,
                Input = text
            };

            try
            {
                using var response = await _httpClient.PostAsJsonAsync(NormalizeRelativePath(_options.EmbeddingsPath), request, JsonOptions, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError(
                        "MedGemma embeddings call failed against {BaseUrl}{Path} with status {StatusCode}: {Body}",
                        _options.BaseUrl, _options.EmbeddingsPath, (int)response.StatusCode, errorBody);
                    throw new InvalidOperationException(
                        $"Embedding model call to '{_httpClient.BaseAddress}{NormalizeRelativePath(_options.EmbeddingsPath)}' " +
                        $"failed with HTTP {(int)response.StatusCode} {response.StatusCode}: {errorBody}");
                }

                var payload = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(JsonOptions, cancellationToken);
                var vector = payload?.Data?.Count > 0 ? payload.Data[0].Embedding : null;

                if (vector is null || vector.Length == 0)
                    throw new InvalidOperationException("The embedding model returned an empty vector.");

                return vector;
            }
            catch (Exception ex) when (ex is not OperationCanceledException and not InvalidOperationException)
            {
                _logger.LogError(ex, "MedGemma embeddings call failed against {BaseUrl}{Path}", _options.BaseUrl, _options.EmbeddingsPath);
                throw new InvalidOperationException(
                    $"Unable to reach the embedding model at '{_options.BaseUrl}{_options.EmbeddingsPath}'. " +
                    $"Verify the AI:EmbeddingModel setting and network connectivity. Underlying error: {ex.Message}", ex);
            }
        }

        // ---- OpenAI-compatible request/response DTOs (internal wire format only) ----

        private sealed class ChatMessage
        {
            [JsonPropertyName("role")] public string Role { get; set; } = string.Empty;
            [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
        }

        private sealed class ChatCompletionRequest
        {
            [JsonPropertyName("model")] public string Model { get; set; } = string.Empty;
            [JsonPropertyName("messages")] public List<ChatMessage> Messages { get; set; } = new();
            [JsonPropertyName("temperature")] public double Temperature { get; set; }
            [JsonPropertyName("max_tokens")] public int MaxTokens { get; set; }
            [JsonPropertyName("stream")] public bool Stream { get; set; } = false;
        }

        private sealed class ChatChoice
        {
            [JsonPropertyName("message")] public ChatMessage? Message { get; set; }
        }

        private sealed class ChatCompletionResponse
        {
            [JsonPropertyName("choices")] public List<ChatChoice>? Choices { get; set; }
        }

        private sealed class EmbeddingRequest
        {
            [JsonPropertyName("model")] public string Model { get; set; } = string.Empty;
            [JsonPropertyName("input")] public string Input { get; set; } = string.Empty;
        }

        private sealed class EmbeddingData
        {
            [JsonPropertyName("embedding")] public float[] Embedding { get; set; } = Array.Empty<float>();
        }

        private sealed class EmbeddingResponse
        {
            [JsonPropertyName("data")] public List<EmbeddingData>? Data { get; set; }
        }
    }
}
