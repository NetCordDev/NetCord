namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Webhook> CreateWebhookAsync(ulong channelId, WebhookProperties webhookProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<WebhookProperties>(webhookProperties, Serialization.Default.WebhookProperties))
            return Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/channels/{channelId}/webhooks", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);
    }

    public async Task<IReadOnlyDictionary<ulong, Webhook>> GetChannelWebhooksAsync(ulong channelId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/webhooks", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhookArray).ConfigureAwait(false)).ToDictionary(w => w.Id, w => Webhook.CreateFromJson(w, this));

    public async Task<IReadOnlyDictionary<ulong, Webhook>> GetGuildWebhooksAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/webhooks", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhookArray).ConfigureAwait(false)).ToDictionary(w => w.Id, w => Webhook.CreateFromJson(w, this));

    public async Task<Webhook> GetWebhookAsync(ulong webhookId, RequestProperties? properties = null)
        => Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}", null, new(webhookId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);

    public async Task<Webhook> GetWebhookWithTokenAsync(ulong webhookId, string webhookToken, RequestProperties? properties = null)
        => Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}/{webhookToken}", null, new(webhookId, webhookToken), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);

    public async Task<Webhook> ModifyWebhookAsync(ulong webhookId, Action<WebhookOptions> action, RequestProperties? properties = null)
    {
        WebhookOptions webhookOptions = new();
        action(webhookOptions);
        using (HttpContent content = new JsonContent<WebhookOptions>(webhookOptions, Serialization.Default.WebhookOptions))
            return Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/webhooks/{webhookId}", null, new(webhookId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);
    }

    public async Task<Webhook> ModifyWebhookWithTokenAsync(ulong webhookId, string webhookToken, Action<WebhookOptions> action, RequestProperties? properties = null)
    {
        WebhookOptions webhookOptions = new();
        action(webhookOptions);
        using (HttpContent content = new JsonContent<WebhookOptions>(webhookOptions, Serialization.Default.WebhookOptions))
            return Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, content, $"/webhooks/{webhookId}/{webhookToken}", null, new(webhookId, webhookToken), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonWebhook).ConfigureAwait(false), this);
    }

    public Task DeleteWebhookAsync(ulong webhookId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}", null, new(webhookId), properties);

    public Task DeleteWebhookWithTokenAsync(ulong webhookId, string webhookToken, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{webhookToken}", null, new(webhookId, webhookToken), properties);

    public async Task<RestMessage?> ExecuteWebhookAsync(ulong webhookId, string webhookToken, WebhookMessageProperties message, bool wait = false, ulong? threadId = null, RequestProperties? properties = null)
    {
        using (HttpContent content = message.Serialize())
        {
            if (wait)
                return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/webhooks/{webhookId}/{webhookToken}", threadId.HasValue ? $"?wait=True&thread_id={threadId.GetValueOrDefault()}" : $"?wait=True", new(webhookId, webhookToken), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
            else
            {
                await SendRequestAsync(HttpMethod.Post, content, $"/webhooks/{webhookId}/{webhookToken}", threadId.HasValue ? $"?thread_id={threadId.GetValueOrDefault()}" : null, new(webhookId, webhookToken), properties).ConfigureAwait(false);
                return null;
            }
        }
    }

    public async Task<RestMessage> GetWebhookMessageAsync(ulong webhookId, string webhookToken, ulong messageId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", null, new(webhookId, webhookToken), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);

    public async Task<RestMessage> ModifyWebhookMessageAsync(ulong webhookId, string webhookToken, ulong messageId, Action<MessageOptions> action, ulong? threadId = null, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Serialize())
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", threadId.HasValue ? $"?thread_id={threadId.GetValueOrDefault()}" : null, new(webhookId, webhookToken), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
    }

    public Task DeleteWebhookMessageAsync(ulong webhookId, string webhookToken, ulong messageId, ulong? threadId = null, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", threadId.HasValue ? $"?thread_id={threadId.GetValueOrDefault()}" : null, new(webhookId, webhookToken), properties);
}
