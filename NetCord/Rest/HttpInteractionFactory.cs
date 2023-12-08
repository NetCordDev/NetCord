using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord.Rest;

public static class HttpInteractionFactory
{
    public static async ValueTask<IInteraction> CreateAsync(Stream body, RestClient client, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, CancellationToken cancellationToken = default)
        => IInteraction.CreateFromJson((await JsonSerializer.DeserializeAsync(body, JsonInteraction.JsonInteractionSerializerContext.WithOptions.JsonInteraction, cancellationToken).ConfigureAwait(false))!, sendResponseAsync, client);

    public static IInteraction Create(Stream body, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client)
        => IInteraction.CreateFromJson(JsonSerializer.Deserialize(body, JsonInteraction.JsonInteractionSerializerContext.WithOptions.JsonInteraction)!, sendResponseAsync, client);

    public static IInteraction Create(ReadOnlySpan<byte> body, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client)
        => IInteraction.CreateFromJson(JsonSerializer.Deserialize(body, JsonInteraction.JsonInteractionSerializerContext.WithOptions.JsonInteraction)!, sendResponseAsync, client);
}
