using System.Diagnostics;

namespace Infrastructure.LLM;

public sealed class FallbackLlmClient : ILLMClient
{
    private readonly IReadOnlyList<ILLMClient> _clients;
    private readonly Func<CancellationToken, Task>? _onTryingAnotherModel;
    public string Provider => "Fallback";
    public string Model => "Fallback";

    public FallbackLlmClient(
        IEnumerable<ILLMClient> clients,
        Func<CancellationToken, Task>? onTryingAnotherModel = null)
    {
        _clients = clients.ToList();
        _onTryingAnotherModel = onTryingAnotherModel;
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

        for (var index = 0; index < _clients.Count; index++)
        {
            var client = _clients[index];
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
                Console.WriteLine($"[LLM] Falling back from: {client.Provider} / {client.Model}");

                var hasNext = index < _clients.Count - 1;
                if (hasNext && _onTryingAnotherModel is not null)
                {
                    await _onTryingAnotherModel(cancellationToken);
                }
            }
        }

        throw new InvalidOperationException(
            "All configured LLM providers and models failed.",
            lastException);
    }
}