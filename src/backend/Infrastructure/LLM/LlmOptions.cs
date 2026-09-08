namespace Infrastructure.LLM;

public class LlmOptions
{
    public int MaxOutputTokens { get; init; } = 500;

    public string ThinkingLevel { get; init; } = "minimal";

    public string? ReasoningEffort { get; set; }

    public List<LlmProviderOptions> Providers { get; init; } = [];
}