using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.LLM;

public class LlmClientFactory
{
    private readonly LlmOptions _options;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public LlmClientFactory(
        IOptions<LlmOptions> options,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public ILLMClient Create()
    {
        var clients = new List<ILLMClient>();

        foreach (var provider in _options.Providers)
        {
            if (string.IsNullOrWhiteSpace(provider.Name))
            {
                continue;
            }

            foreach (var model in provider.Models)
            {
                if (string.IsNullOrWhiteSpace(model))
                {
                    continue;
                }

                clients.Add(
                    CreateClient(
                        provider,
                        model));
            }
        }

        if (clients.Count == 0)
        {
            throw new InvalidOperationException(
                "No LLM providers or models are configured.");
        }

        return new FallbackLlmClient(clients);
    }

    private ILLMClient CreateClient(
        LlmProviderOptions provider,
        string model)
    {
        var providerName =
            provider.Name
                .Trim()
                .ToLowerInvariant();

        return providerName switch
        {
            "google" =>
                CreateGoogle(
                    provider,
                    model),

            "groq" =>
                CreateGroq(
                    provider,
                    model),

            "openrouter" =>
                CreateOpenRouter(
                    provider,
                    model),

            _ =>
                throw new InvalidOperationException(
                    $"Unsupported LLM provider: {provider.Name}")
        };
    }

    private GoogleClient CreateGoogle(
        LlmProviderOptions provider,
        string model)
    {
        var httpClient =
            _httpClientFactory.CreateClient(
                "Google");

        httpClient.Timeout =
            TimeSpan.FromSeconds(
                provider.TimeoutSeconds);

        return new GoogleClient(
            httpClient,
            Options.Create(
                new LlmOptions
                {
                    MaxOutputTokens =
                        _options.MaxOutputTokens,

                    ThinkingLevel =
                        _options.ThinkingLevel,

                    Providers =
                        _options.Providers
                }),
            _configuration,
            model);
    }

    private GroqClient CreateGroq(
        LlmProviderOptions provider,
        string model)
    {
        var httpClient =
            _httpClientFactory.CreateClient(
                "Groq");

        httpClient.Timeout =
            TimeSpan.FromSeconds(
                provider.TimeoutSeconds);

        return new GroqClient(
            httpClient,
            Options.Create(
                new LlmOptions
                {
                    MaxOutputTokens =
                        _options.MaxOutputTokens,

                    ThinkingLevel =
                        _options.ThinkingLevel,

                    Providers =
                        _options.Providers
                }),
            _configuration,
            model);
    }

    private OpenRouterClient CreateOpenRouter(
        LlmProviderOptions provider,
        string model)
    {
        var httpClient =
            _httpClientFactory.CreateClient(
                "OpenRouter");

        httpClient.Timeout =
            TimeSpan.FromSeconds(
                provider.TimeoutSeconds);

        return new OpenRouterClient(
            httpClient,
            Options.Create(
                new LlmOptions
                {
                    MaxOutputTokens =
                        _options.MaxOutputTokens,

                    ThinkingLevel =
                        _options.ThinkingLevel,

                    Providers =
                        _options.Providers
                }),
            _configuration,
            model);
    }
}