using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;

namespace Api;

internal static class ServerSentEvents
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static void Prepare(HttpResponse response)
    {
        response.Headers.CacheControl = "no-cache, no-transform";
        response.Headers.Append("X-Accel-Buffering", "no");
        response.ContentType = "text/event-stream";
        response.HttpContext.Features.Get<IHttpResponseBodyFeature>()
            ?.DisableBuffering();
    }

    public static Task WritePhaseAsync(
        HttpResponse response,
        string phase,
        CancellationToken cancellationToken)
    {
        return WriteAsync(response, "phase", phase, cancellationToken);
    }

    public static Task WriteJsonAsync(
        HttpResponse response,
        string eventName,
        object payload,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        return WriteAsync(response, eventName, json, cancellationToken);
    }

    private static async Task WriteAsync(
        HttpResponse response,
        string eventName,
        string data,
        CancellationToken cancellationToken)
    {
        await response.WriteAsync($"event: {eventName}\n", cancellationToken);
        await response.WriteAsync($"data: {data}\n\n", cancellationToken);
        await response.Body.FlushAsync(cancellationToken);
    }
}
