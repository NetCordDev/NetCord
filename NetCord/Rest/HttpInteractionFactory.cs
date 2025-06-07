using System.Text.Json;

namespace NetCord.Rest;

public static class HttpInteractionFactory
{
    public static async ValueTask<IInteraction> CreateAsync(Stream body, RestClient client, InteractionResponseDelegate sendResponseAsync, CancellationToken cancellationToken = default)
        => IInteraction.CreateFromJson((await JsonSerializer.DeserializeAsync(body, Serialization.Default.JsonInteraction, cancellationToken).ConfigureAwait(false))!, sendResponseAsync, client);

    public static IInteraction Create(Stream body, InteractionResponseDelegate sendResponseAsync, RestClient client)
        => IInteraction.CreateFromJson(JsonSerializer.Deserialize(body, Serialization.Default.JsonInteraction)!, sendResponseAsync, client);

    public static IInteraction Create(ReadOnlySpan<byte> body, InteractionResponseDelegate sendResponseAsync, RestClient client)
        => IInteraction.CreateFromJson(JsonSerializer.Deserialize(body, Serialization.Default.JsonInteraction)!, sendResponseAsync, client);
}
