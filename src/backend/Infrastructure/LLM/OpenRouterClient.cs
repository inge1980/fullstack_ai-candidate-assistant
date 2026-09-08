using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.LLM;

public class OpenRouterClient : ILLMClient
{
    private readonly HttpClient _httpClient;
    private readonly LlmOptions _options;
    private readonly IConfiguration _configuration;
    private readonly string _model;
    public string Provider => "OpenRouter";
    public string Model => _model;

    public OpenRouterClient(
        HttpClient httpClient,
        IOptions<LlmOptions> options,
        IConfiguration configuration,
        string model)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _configuration = configuration;
        _model = model;
    }

    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OpenRouter:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new LlmProviderException(
                "OpenRouter",
                "OpenRouter API key is missing.");
        }

        if (string.IsNullOrWhiteSpace(_model))
        {
            throw new LlmProviderException(
                "OpenRouter",
                "OpenRouter model is missing.");
        }

        var baseUrl =
            _configuration["OpenRouter:BaseUrl"]
            ?? throw new LlmProviderException(
                "OpenRouter",
                "OpenRouter base URL is missing.");

        var url = $"{baseUrl}/chat/completions";

        var request =
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
                max_tokens = _options.MaxOutputTokens
            };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            url)
        {
            Content = JsonContent.Create(request)
        };

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                apiKey);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response =
                await _httpClient.SendAsync(
                    httpRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

            stopwatch.Stop();
            Console.WriteLine($"[LLM] OpenRouter HTTP: {stopwatch.ElapsedMilliseconds} ms");
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
                    "OpenRouter",
                    $"OpenRouter request failed ({statusCode}): {responseBody}",
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
                    "OpenRouter",
                    "OpenRouter returned an empty response.");
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
                "OpenRouter",
                $"OpenRouter request timed out after {stopwatch.ElapsedMilliseconds} ms.",
                isTransient: true,
                innerException: ex);
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();

            throw new LlmProviderException(
                "OpenRouter",
                "OpenRouter network request failed.",
                isTransient: true,
                innerException: ex);
        }
    }
}