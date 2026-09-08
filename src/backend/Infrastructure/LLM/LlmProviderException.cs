namespace Infrastructure.LLM;

public sealed class LlmProviderException : Exception
{
    public string Provider { get; }

    public int? StatusCode { get; }

    public bool IsTransient { get; }

    public LlmProviderException(
        string provider,
        string message,
        int? statusCode = null,
        bool isTransient = false,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Provider = provider;
        StatusCode = statusCode;
        IsTransient = isTransient;
    }
}