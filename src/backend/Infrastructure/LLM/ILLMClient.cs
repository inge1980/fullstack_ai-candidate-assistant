namespace Infrastructure.LLM;

public interface ILLMClient
{
    string Provider { get; }

    string Model { get; }

    Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}