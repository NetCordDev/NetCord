namespace NetCord.Rest;

public partial class RestClient
{
    public Task SendInteractionResponseAsync(Snowflake interactionId, string interactionToken, InteractionCallback callback, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Post, $"/interactions/{interactionId}/{interactionToken}/callback", callback.Build(), properties);

    public async Task<RestMessage> GetInteractionResponseAsync(Snowflake applicationId, string interactionToken, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", new RateLimits.Route(RateLimits.RouteParameter.Interaction), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> ModifyInteractionResponseAsync(Snowflake applicationId, string interactionToken, Action<MessageOptions> action, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", new(RateLimits.RouteParameter.Interaction), messageOptions.Build(), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);
    }

    public Task DeleteInteractionResponseAsync(Snowflake applicationId, string interactionToken, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", new RateLimits.Route(RateLimits.RouteParameter.Interaction), properties);

    public async Task<RestMessage> SendInteractionFollowupMessageAsync(Snowflake applicationId, string interactionToken, InteractionMessageProperties message, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/webhooks/{applicationId}/{interactionToken}", new(RateLimits.RouteParameter.Interaction), message.Build(), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> GetInteractionFollowupMessageAsync(Snowflake applicationId, string interactionToken, Snowflake messageId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", new RateLimits.Route(RateLimits.RouteParameter.Interaction), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> ModifyInteractionFollowupMessageAsync(Snowflake applicationId, string interactionToken, Snowflake messageId, Action<MessageOptions> action, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", new(RateLimits.RouteParameter.Interaction), messageOptions.Build(), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);
    }

    public Task DeleteInteractionFollowupMessageAsync(Snowflake applicationId, string interactionToken, Snowflake messageId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", new RateLimits.Route(RateLimits.RouteParameter.Interaction), properties);
}