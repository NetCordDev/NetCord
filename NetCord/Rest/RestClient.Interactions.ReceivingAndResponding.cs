namespace NetCord.Rest;

public partial class RestClient
{
    public Task SendInteractionResponseAsync(Snowflake interactionId, string interactionToken, InteractionCallback callback, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Post, callback.Build(), $"/interactions/{interactionId}/{interactionToken}/callback", options);

    public async Task<RestMessage> GetInteractionResponseAsync(Snowflake applicationId, string interactionToken, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> ModifyInteractionResponseAsync(Snowflake applicationId, string interactionToken, Action<MessageOptions> action, RequestProperties? options = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, messageOptions.Build(), $"/webhooks/{applicationId}/{interactionToken}/messages/@original", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);
    }

    public Task DeleteInteractionResponseAsync(Snowflake applicationId, string interactionToken, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", options);

    public async Task<RestMessage> SendInteractionFollowupMessageAsync(Snowflake applicationId, string interactionToken, InteractionMessageProperties message, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, message.Build(), $"/webhooks/{applicationId}/{interactionToken}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> GetInteractionFollowupMessageAsync(Snowflake applicationId, string interactionToken, Snowflake messageId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> ModifyInteractionFollowupMessageAsync(Snowflake applicationId, string interactionToken, Snowflake messageId, Action<MessageOptions> action, RequestProperties? options = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, messageOptions.Build(), $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);
    }

    public Task DeleteInteractionFollowupMessageAsync(Snowflake applicationId, string interactionToken, Snowflake messageId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", options);
}