namespace NetCord.Rest;

public partial class RestClient
{
    public async Task SendInteractionResponseAsync(ulong interactionId, string interactionToken, InteractionCallback callback, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = callback.Serialize())
            await SendRequestAsync(HttpMethod.Post, content, $"/interactions/{interactionId}/{interactionToken}/callback", null, new(interactionId, interactionToken), properties, false).ConfigureAwait(false);
    }

    [GenerateAlias([typeof(Interaction)], nameof(Interaction.ApplicationId), nameof(Interaction.Token))]
    public async Task<RestMessage> GetInteractionResponseAsync(ulong applicationId, string interactionToken, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    [GenerateAlias([typeof(Interaction)], nameof(Interaction.ApplicationId), nameof(Interaction.Token))]
    public async Task<RestMessage> ModifyInteractionResponseAsync(ulong applicationId, string interactionToken, Action<MessageOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(Interaction)], nameof(Interaction.ApplicationId), nameof(Interaction.Token))]
    public Task DeleteInteractionResponseAsync(ulong applicationId, string interactionToken, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/@original", null, new(applicationId, interactionToken), properties, false);

    [GenerateAlias([typeof(Interaction)], nameof(Interaction.ApplicationId), nameof(Interaction.Token))]
    public async Task<RestMessage> SendInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, InteractionMessageProperties message, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = message.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/webhooks/{applicationId}/{interactionToken}", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(Interaction)], nameof(Interaction.ApplicationId), nameof(Interaction.Token))]
    public async Task<RestMessage> GetInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    [GenerateAlias([typeof(Interaction)], nameof(Interaction.ApplicationId), nameof(Interaction.Token))]
    public async Task<RestMessage> ModifyInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, ulong messageId, Action<MessageOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", null, new(applicationId, interactionToken), properties, false).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(Interaction)], nameof(Interaction.ApplicationId), nameof(Interaction.Token))]
    public Task DeleteInteractionFollowupMessageAsync(ulong applicationId, string interactionToken, ulong messageId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{applicationId}/{interactionToken}/messages/{messageId}", null, new(applicationId, interactionToken), properties, false);
}
