using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    /// <summary>
    /// Creates a new webhook, and returns an <see cref="IncomingWebhook"/> object on success.
    /// </summary>
    /// <remarks>
    /// Requires the <see cref="Permissions.ManageWebhooks"/> permission, and fires a <see cref="GatewayClient.WebhooksUpdate"/> event.
    /// </remarks>
    /// <param name="channelId">The ID of the channel to create the webhook in.</param>
    /// <param name="webhookProperties">Properties to customize the webhook's appearance.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(ForumGuildChannel)], nameof(ForumGuildChannel.Id))]
    [GenerateAlias([typeof(TextGuildChannel)], nameof(TextGuildChannel.Id))]
    public async Task<IncomingWebhook> CreateWebhookAsync(ulong channelId, WebhookProperties webhookProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<WebhookProperties>(webhookProperties, Serialization.Default.WebhookProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/webhooks", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Returns a list of channel <see cref="Webhook"/> objects.
    /// </summary>
    /// <param name="channelId">The ID of the channel to retrieve webhooks for.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(ForumGuildChannel)], nameof(ForumGuildChannel.Id))]
    [GenerateAlias([typeof(TextGuildChannel)], nameof(TextGuildChannel.Id))]
    public async Task<IReadOnlyList<Webhook>> GetChannelWebhooksAsync(ulong channelId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/webhooks", null, new(channelId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhookArray).ConfigureAwait(false)).Select(w => Webhook.CreateFromJson(w, this)).ToArray();

    /// <summary>
    /// Retrieves a list of webhooks for the specified guild ID.
    /// </summary>
    /// <remarks>
    /// Requires the <see cref="Permissions.ManageWebhooks"/> permission.
    /// </remarks>
    /// <param name="guildId">The ID of the guild to retrieve webhooks for.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<Webhook>> GetGuildWebhooksAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/webhooks", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhookArray).ConfigureAwait(false)).Select(w => Webhook.CreateFromJson(w, this)).ToArray();

    /// <summary>
    /// Retrieves the new <see cref="Webhook"/> object for the given ID.
    /// </summary>
    /// <remarks>
    /// Requires the <see cref="Permissions.ManageWebhooks"/> permission unless the application making the request owns the webhook.
    /// </remarks>
    /// <param name="webhookId">The ID of the webhook to retrieve.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(Webhook)], nameof(Webhook.Id), Cast = true)]
    public async Task<Webhook> GetWebhookAsync(ulong webhookId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}", null, new(webhookId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);

    /// <summary>
    /// Retrieves the new <see cref="Webhook"/> object for the given ID.
    /// </summary>
    /// <remarks>
    ///     <para>Requires the <see cref="Permissions.ManageWebhooks"/> permission unless the application making the request owns the webhook.</para>
    /// </remarks>
    /// <param name="webhookId">The ID of the webhook to retrieve.</param>
    /// <param name="webhookToken">The token to use for authorization.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IncomingWebhook)], nameof(IncomingWebhook.Id), nameof(IncomingWebhook.Token), Cast = true, TypeNameOverride = nameof(Webhook))]
    [GenerateAlias([typeof(WebhookClient)], nameof(WebhookClient.Id), nameof(WebhookClient.Token), TypeNameOverride = $"{nameof(Webhook)}WithToken")]
    public async Task<Webhook> GetWebhookWithTokenAsync(ulong webhookId, string webhookToken, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}/{webhookToken}", null, new(webhookId, webhookToken), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);

    /// <summary>
    /// Modifies a webhook, returning the updated <see cref="Webhook"/> object on success.
    /// </summary>
    /// <remarks>
    /// Requires the <see cref="Permissions.ManageWebhooks"/> permission, and fires a <see cref="GatewayClient.WebhooksUpdate"/> event.
    /// </remarks>
    /// <param name="webhookId">The ID of the webhook to modify.</param>
    /// <param name="action">The modification to perform on the webhook.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(Webhook)], nameof(Webhook.Id), Cast = true)]
    public async Task<Webhook> ModifyWebhookAsync(ulong webhookId, Action<WebhookOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        WebhookOptions webhookOptions = new();
        action(webhookOptions);
        using (HttpContent content = new JsonContent<WebhookOptions>(webhookOptions, Serialization.Default.WebhookOptions))
            return Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/webhooks/{webhookId}", null, new(webhookId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Modifies a webhook, returning the updated <see cref="Webhook"/> object on success.
    /// </summary>
    /// <remarks>
    ///     <para>Requires the <see cref="Permissions.ManageWebhooks"/> permission, and fires a <see cref="GatewayClient.WebhooksUpdate"/> event.</para>
    ///     <para>Does not allow modifiying the <see cref="WebhookOptions.ChannelId"/> property.</para>
    /// </remarks>
    /// <param name="webhookId">The ID of the webhook to modify.</param>
    /// <param name="webhookToken">The token to use for authorization.</param>
    /// <param name="action">The modification to perform on the webhook.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IncomingWebhook)], nameof(IncomingWebhook.Id), nameof(IncomingWebhook.Token), Cast = true, TypeNameOverride = nameof(Webhook))]
    [GenerateAlias([typeof(WebhookClient)], nameof(WebhookClient.Id), nameof(WebhookClient.Token), TypeNameOverride = $"{nameof(Webhook)}WithToken")]
    public async Task<Webhook> ModifyWebhookWithTokenAsync(ulong webhookId, string webhookToken, Action<WebhookOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        WebhookOptions webhookOptions = new();
        action(webhookOptions);
        using (HttpContent content = new JsonContent<WebhookOptions>(webhookOptions, Serialization.Default.WebhookOptions))
            return Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/webhooks/{webhookId}/{webhookToken}", null, new(webhookId, webhookToken), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Deletes a webhook permanently.
    /// </summary>
    /// <remarks>
    /// Requires the <see cref="Permissions.ManageWebhooks"/> permission, and fires a <see cref="GatewayClient.WebhooksUpdate"/> event.
    /// </remarks>
    /// <param name="webhookId">The ID of the webhook to delete.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(Webhook)], nameof(Webhook.Id))]
    public Task DeleteWebhookAsync(ulong webhookId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}", null, new(webhookId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Deletes a webhook permanently.
    /// </summary>
    /// <remarks>
    /// Requires the <see cref="Permissions.ManageWebhooks"/> permission, and fires a <see cref="GatewayClient.WebhooksUpdate"/> event.
    /// </remarks>
    /// <param name="webhookId">The ID of the webhook to delete.</param>
    /// <param name="webhookToken">The token to use for authorization.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IncomingWebhook)], nameof(IncomingWebhook.Id), nameof(IncomingWebhook.Token), TypeNameOverride = nameof(Webhook))]
    [GenerateAlias([typeof(WebhookClient)], nameof(WebhookClient.Id), nameof(WebhookClient.Token), TypeNameOverride = $"{nameof(Webhook)}WithToken")]
    public Task DeleteWebhookWithTokenAsync(ulong webhookId, string webhookToken, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{webhookToken}", null, new(webhookId, webhookToken), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Executes a webhook with the given parameters, optionally returning the message result.
    /// </summary>
    /// <param name="webhookId">The ID of the webhook to execute.</param>
    /// <param name="webhookToken">The token to use for authorization.</param>
    /// <param name="message">The message to send through the webhook.</param>
    /// <param name="wait">If set, the returned <see cref="RestMessage"/> will contain the sent message.</param>
    /// <param name="threadId">Optional ID of the thread to send the message in, within the webhook's target channel. Can be <see langword="null"/>.</param>
    /// <param name="withComponents">
    ///     <para>Whether to respect the <paramref name="message"/>'s <see cref="WebhookMessageProperties.Components"/>.</para>
    ///     <para>When enabled, allows application-owned webhooks to use all components, and non-owned webhooks to use non-interactive components.</para>
    /// </param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IncomingWebhook)], nameof(IncomingWebhook.Id), nameof(IncomingWebhook.Token), TypeNameOverride = nameof(Webhook))]
    [GenerateAlias([typeof(WebhookClient)], nameof(WebhookClient.Id), nameof(WebhookClient.Token), TypeNameOverride = nameof(Webhook))]
    public async Task<RestMessage?> ExecuteWebhookAsync(ulong webhookId, string webhookToken, WebhookMessageProperties message, bool wait = false, ulong? threadId = null, bool withComponents = true, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = message.Serialize())
        {
            if (wait)
                return new(await (await SendRequestAsync(HttpMethod.Post,
                                                         content,
                                                         $"/webhooks/{webhookId}/{webhookToken}",
                                                         threadId.HasValue
                                                            ? (withComponents
                                                                ? $"?wait=True&with_components=True&thread_id={threadId.GetValueOrDefault()}"
                                                                : $"?wait=True&thread_id={threadId.GetValueOrDefault()}")
                                                            : (withComponents
                                                                ? $"?wait=True&with_components=True"
                                                                : $"?wait=True"),
                                                         new(webhookId, webhookToken),
                                                         properties,
                                                         cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
            else
            {
                await SendRequestAsync(HttpMethod.Post,
                                       content,
                                       $"/webhooks/{webhookId}/{webhookToken}",
                                       threadId.HasValue
                                            ? (withComponents
                                                 ? $"?with_components=True&thread_id={threadId.GetValueOrDefault()}"
                                                 : $"?thread_id={threadId.GetValueOrDefault()}")
                                            : (withComponents
                                                 ? "?with_components=True"
                                                 : null),
                                       new(webhookId, webhookToken),
                                       properties,
                                       cancellationToken: cancellationToken).ConfigureAwait(false);
                return null;
            }
        }
    }

    /// <summary>
    /// Returns a previously-sent webhook message from the given token.
    /// </summary>
    /// <param name="webhookId">The ID of the webhook to retrieve the message from.</param>
    /// <param name="webhookToken">The token to use for authorization.</param>
    /// <param name="messageId">The ID of the message to retrieve.</param>
    /// <param name="threadId">The ID of the thread the message is in.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IncomingWebhook)], nameof(IncomingWebhook.Id), nameof(IncomingWebhook.Token), TypeNameOverride = nameof(Webhook))]
    [GenerateAlias([typeof(WebhookClient)], nameof(WebhookClient.Id), nameof(WebhookClient.Token), TypeNameOverride = nameof(Webhook))]
    public async Task<RestMessage> GetWebhookMessageAsync(ulong webhookId, string webhookToken, ulong messageId, ulong? threadId = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", threadId.HasValue ? $"?thread_id={threadId.GetValueOrDefault()}" : null, new(webhookId, webhookToken), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    /// <summary>
    /// Modifies a previously-sent webhook message from the given token.
    /// </summary>
    /// <param name="webhookId">The ID of the webhook to modify the message with.</param>
    /// <param name="webhookToken">The token to use for authorization.</param>
    /// <param name="messageId">The ID of the message to modify.</param>
    /// <param name="action">The modification to apply to the message.</param>
    /// <param name="threadId">The ID of the thread the message is in.</param>
    /// <param name="withComponents">
    ///     <para>Whether to respect the modification's <see cref="MessageOptions.Components"/>.</para>
    ///     <para>When enabled, allows application-owned webhooks to use all components, and non-owned webhooks to use non-interactive components.</para>
    /// </param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IncomingWebhook)], nameof(IncomingWebhook.Id), nameof(IncomingWebhook.Token), TypeNameOverride = nameof(Webhook))]
    [GenerateAlias([typeof(WebhookClient)], nameof(WebhookClient.Id), nameof(WebhookClient.Token), TypeNameOverride = nameof(Webhook))]
    public async Task<RestMessage> ModifyWebhookMessageAsync(ulong webhookId, string webhookToken, ulong messageId, Action<MessageOptions> action, ulong? threadId = null, bool withComponents = true, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Patch,
                                                     content,
                                                     $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}",
                                                     threadId.HasValue
                                                        ? (withComponents
                                                            ? $"?with_components=True&thread_id={threadId.GetValueOrDefault()}"
                                                            : $"?thread_id={threadId.GetValueOrDefault()}")
                                                        : (withComponents
                                                            ? "?with_components=True"
                                                            : null),
                                                     new(webhookId, webhookToken),
                                                     properties,
                                                     cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Deletes a previously-sent webhook message permanently.
    /// </summary>
    /// <param name="webhookId">The ID of the webhook to delete the message with.</param>
    /// <param name="webhookToken">The token to use for authorization.</param>
    /// <param name="messageId">The ID of the message to delete.</param>
    /// <param name="threadId">The ID of the thread the message is in.</param>
    /// <param name="properties">Optional properties to customize the request, can be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation before it completes.</param>
    [GenerateAlias([typeof(IncomingWebhook)], nameof(IncomingWebhook.Id), nameof(IncomingWebhook.Token), TypeNameOverride = nameof(Webhook))]
    [GenerateAlias([typeof(WebhookClient)], nameof(WebhookClient.Id), nameof(WebhookClient.Token), TypeNameOverride = nameof(Webhook))]
    public Task DeleteWebhookMessageAsync(ulong webhookId, string webhookToken, ulong messageId, ulong? threadId = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", threadId.HasValue ? $"?thread_id={threadId.GetValueOrDefault()}" : null, new(webhookId, webhookToken), properties, cancellationToken: cancellationToken);
}
