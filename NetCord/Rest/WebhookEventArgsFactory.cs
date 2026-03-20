using System.Text.Json;

using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public static class WebhookEventArgsFactory
{
    public static ValueTask<JsonWebhookEventArgs> CreateJsonAsync(Stream body, CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync(body, Serialization.Default.JsonWebhookEventArgs, cancellationToken)!;

    public static JsonWebhookEventArgs CreateJson(Stream body)
        => JsonSerializer.Deserialize(body, Serialization.Default.JsonWebhookEventArgs)!;

    public static JsonWebhookEventArgs CreateJson(ReadOnlySpan<byte> body)
        => JsonSerializer.Deserialize(body, Serialization.Default.JsonWebhookEventArgs)!;
}
