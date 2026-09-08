namespace Infrastructure.LLM;

public class LlmProviderOptions
{
    public string Name { get; init; } = string.Empty;

    public List<string> Models { get; init; } = [];

    public int TimeoutSeconds { get; init; } = 20;
}