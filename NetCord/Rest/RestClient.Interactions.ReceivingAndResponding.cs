namespace NetCord;

public partial class RestClient
{
    public Task SendInteractionResponseAsync(DiscordId interactionId, string interactionToken, InteractionCallback callback, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Post, callback.Build(), $"/interactions/{interactionId}/{interactionToken}/callback", options);

    public async Task<RestMessage> GetInteractionResponseAsync(DiscordId applicationId, string interactionToken, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> ModifyInteractionResponseAsync(DiscordId applicationId, string interactionToken, Action<MessageOptions> action, RequestProperties? options = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, messageOptions.Build(), $"/webhooks/{applicationId}/{interactionToken}/messages/@original", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);
    }

    public Task DeleteInteractionResponseAsync(DiscordId applicationId, string interactionToken, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", options);

    public async Task<RestMessage> SendInteractionFollowupMessageAsync(DiscordId applicationId, string interactionToken, InteractionMessageProperties message, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, message.Build(), $"/webhooks/{applicationId}/{interactionToken}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> GetInteractionFollowupMessageAsync(DiscordId applicationId, string interactionToken, DiscordId messageId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> ModifyInteractionFollowupMessageAsync(DiscordId applicationId, string interactionToken, DiscordId messageId, Action<MessageOptions> action, RequestProperties? options = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(messageOptions), $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);
    }

    public Task DeleteInteractionFollowupMessageAsync(DiscordId applicationId, string interactionToken, DiscordId messageId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", options);
}