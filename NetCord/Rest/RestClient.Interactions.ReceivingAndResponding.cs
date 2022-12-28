namespace NetCord.Rest;

public partial class RestClient
{
    public async Task SendInteractionResponseAsync(ulong interactionId, string interactionToken, InteractionCallback callback, RequestProperties? properties = null)
    {
        using (HttpContent content = callback.Build())
            await SendRequestWithoutRateLimitAsync(HttpMethod.Post, $"/interactions/{interactionId}/{interactionToken}/callback", content, properties).ConfigureAwait(false);
    }

    public async Task<RestMessage> GetInteractionResponseAsync(ulong applicationId, string interactionToken, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", new RateLimits.Route(RateLimits.RouteParameter.Interaction, globalRateLimit: false), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);

    public async Task<RestMessage> ModifyInteractionResponseAsync(ulong applicationId, string interactionToken, Action<MessageOptions> action, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Build())
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", new(RateLimits.RouteParameter.Interaction, globalRateLimit: false), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
    }

    public Task DeleteInteractionResponseAsync(ulong applicationId, string interactionToken, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", new RateLimits.Route(RateLimits.RouteParameter.Interaction, globalRateLimit: false), properties);

    public async Task<RestMessage> SendInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, InteractionMessageProperties message, RequestProperties? properties = null)
    {
        using (HttpContent content = message.Build())
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/webhooks/{applicationId}/{interactionToken}", new(RateLimits.RouteParameter.Interaction, globalRateLimit: false), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
    }

    public async Task<RestMessage> GetInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, ulong messageId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", new RateLimits.Route(RateLimits.RouteParameter.Interaction, globalRateLimit: false), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);

    public async Task<RestMessage> ModifyInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, ulong messageId, Action<MessageOptions> action, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Build())
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", new(RateLimits.RouteParameter.Interaction, globalRateLimit: false), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
    }

    public Task DeleteInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, ulong messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", new RateLimits.Route(RateLimits.RouteParameter.Interaction, globalRateLimit: false), properties);
}
