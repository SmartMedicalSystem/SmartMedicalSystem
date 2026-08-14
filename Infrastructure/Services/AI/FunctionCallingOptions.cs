namespace Infrastructure.Services.AI;

public sealed class FunctionCallingOptions
{
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";
    public string ChatCompletionsPath { get; set; } = "/chat/completions";
    public string ChatModel { get; set; } = "openai/gpt-oss-20b:free";
    public string? ApiKey { get; set; }
    public double Temperature { get; set; } = 0.2;
    public int MaxOutputTokens { get; set; } = 1024;
    public int TimeoutSeconds { get; set; } = 120;
}
