using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.LLM;

public class CerebrasClient : ILLMClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _model;
    private readonly int _maxOutputTokens;
    public string Provider => "Cerebras";
    public string Model => _model;

    public CerebrasClient(
        HttpClient httpClient,
        IConfiguration configuration,
        string model,
        int maxOutputTokens)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _model = model;
        _maxOutputTokens = maxOutputTokens;
    }

    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Cerebras:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new LlmProviderException(
                "Cerebras",
                "Cerebras API key is missing.");
        }

        if (string.IsNullOrWhiteSpace(_model))
        {
            throw new LlmProviderException(
                "Cerebras",
                "Cerebras model is missing.");
        }

        var baseUrl =
            _configuration["Cerebras:BaseUrl"]
            ?? throw new LlmProviderException(
                "Cerebras",
                "Cerebras base URL is missing.");

        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                $"{baseUrl}/chat/completions");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                apiKey);

        request.Content =
            JsonContent.Create(
                new
                {
                    model = _model,
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    },
                    max_tokens = _maxOutputTokens
                });

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response =
                await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

            stopwatch.Stop();

            Console.WriteLine($"[LLM] Cerebras HTTP: {stopwatch.ElapsedMilliseconds} ms");

            var responseBody =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;

                var transient =
                    statusCode == 408 ||
                    statusCode == 429 ||
                    statusCode >= 500;

                throw new LlmProviderException(
                    "Cerebras",
                    $"Cerebras request failed ({statusCode}): {responseBody}",
                    statusCode,
                    transient);
            }

            using var json = JsonDocument.Parse(responseBody);

            var text =
                json.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

            return text
                ?? throw new LlmProviderException(
                    "Cerebras",
                    "Cerebras returned an empty response.");
        }
        catch (LlmProviderException)
        {
            throw;
        }
        catch (TaskCanceledException ex)
            when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();

            throw new LlmProviderException(
                "Cerebras",
                $"Cerebras request timed out after {stopwatch.ElapsedMilliseconds} ms.",
                isTransient: true,
                innerException: ex);
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();

            throw new LlmProviderException(
                "Cerebras",
                "Cerebras network request failed.",
                isTransient: true,
                innerException: ex);
        }
    }
}