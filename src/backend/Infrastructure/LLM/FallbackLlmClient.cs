using System.Diagnostics;

namespace Infrastructure.LLM;

public sealed class FallbackLlmClient : ILLMClient
{
    private readonly IReadOnlyList<ILLMClient> _clients;
    public string Provider => "Fallback";
    public string Model => "Fallback";

    public FallbackLlmClient(
        IEnumerable<ILLMClient> clients)
    {
        _clients = clients.ToList();
        if (_clients.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one LLM client must be configured.");
        }
    }

    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        Exception? lastException = null;

        foreach (var client in _clients)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[LLM] Trying: {client.Provider} / {client.Model}");

                var result =
                    await client.GenerateAsync(
                        prompt,
                        cancellationToken);

                stopwatch.Stop();
                Console.WriteLine($"[LLM] Succeeded: {client.Provider} / {client.Model} ({stopwatch.ElapsedMilliseconds} ms)");
                return result;
            }
            catch (LlmProviderException ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"[LLM] Failed: {client.Provider} / {client.Model} ({stopwatch.ElapsedMilliseconds} ms) Status={ex.StatusCode} Transient={ex.IsTransient}");
                lastException = ex;
                if (!ex.IsTransient)
                {
                    Console.WriteLine($"[LLM] Falling back from: {client.Provider} / {client.Model}");
                    continue;
                }
                Console.WriteLine($"[LLM] Falling back from: {client.Provider} / {client.Model}");
            }
        }

        throw new InvalidOperationException(
            "All configured LLM providers and models failed.",
            lastException);
    }
}