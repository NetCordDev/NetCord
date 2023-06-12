using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task SendInteractionResponseAsync(ulong interactionId, string interactionToken, InteractionCallback callback, RequestProperties? properties = null)
    {
        using (HttpContent content = callback.Serialize())
            await SendRequestAsync(HttpMethod.Post, content, $"/interactions/{interactionId}/{interactionToken}/callback", null, new(interactionId), properties, false).ConfigureAwait(false);
    }

    public async Task<RestMessage> GetInteractionResponseAsync(ulong applicationId, string interactionToken, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);

    public async Task<RestMessage> ModifyInteractionResponseAsync(ulong applicationId, string interactionToken, Action<MessageOptions> action, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
    }

    public Task DeleteInteractionResponseAsync(ulong applicationId, string interactionToken, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", null, new(applicationId, interactionToken), properties, false);

    public async Task<RestMessage> SendInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, InteractionMessageProperties message, RequestProperties? properties = null)
    {
        using (HttpContent content = message.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/webhooks/{applicationId}/{interactionToken}", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
    }

    public async Task<RestMessage> GetInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, ulong messageId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);

    public async Task<RestMessage> ModifyInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, ulong messageId, Action<MessageOptions> action, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
    }

    public Task DeleteInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", null, new(applicationId, interactionToken), properties, false);
}
