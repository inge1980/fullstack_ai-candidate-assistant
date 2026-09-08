// Text to embedding
using System.Net.Http.Json;

namespace Infrastructure.Embeddings;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;

    private static readonly string Model =
        Environment.GetEnvironmentVariable("OLLAMA_EMBEDDING_MODEL")
        ?? throw new InvalidOperationException(
        "OLLAMA_EMBEDDING_MODEL environment variable is missing.");

    private static readonly string OllamaUrl =
        Environment.GetEnvironmentVariable("OLLAMA_EMBED_URL")
        ?? throw new InvalidOperationException(
        "OLLAMA_EMBED_URL environment variable is missing.");
    
    public EmbeddingService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<float[]> Create(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be empty.",
                nameof(text));
        }

        var request = new
        {
            model = Model,
            input = text
        };

        var response =
            await _httpClient.PostAsJsonAsync(
                OllamaUrl,
                request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<OllamaEmbeddingResponse>();

        if (result?.Embeddings == null ||
            result.Embeddings.Length == 0)
        {
            throw new InvalidOperationException(
                "Ollama returned no embedding.");
        }

        var embedding = result.Embeddings[0];

        if (embedding.Length != 384)
        {
            throw new InvalidOperationException(
                $"Expected a 384-dimensional embedding, " +
                $"but Ollama returned {embedding.Length} dimensions.");
        }

        return embedding;
    }

    private sealed class OllamaEmbeddingResponse
    {
        public float[][] Embeddings { get; set; } = [];
    }
}