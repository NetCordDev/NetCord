namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Webhook> CreateWebhookAsync(Snowflake channelId, WebhookProperties webhookProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(webhookProperties), $"/channels/{channelId}/webhooks", options).ConfigureAwait(false)).ToObject<JsonModels.JsonWebhook>(), this);

    public async Task<IReadOnlyDictionary<Snowflake, Webhook>> GetChannelWebhooksAsync(Snowflake channelId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/webhooks", options).ConfigureAwait(false)).ToObject<JsonModels.JsonWebhook[]>().ToDictionary(w => w.Id, w => new Webhook(w, this));

    public async Task<IReadOnlyDictionary<Snowflake, Webhook>> GetGuildWebhooksAsync(Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/webhooks", options).ConfigureAwait(false)).ToObject<JsonModels.JsonWebhook[]>().ToDictionary(w => w.Id, w => new Webhook(w, this));

    public async Task<Webhook> GetWebhookAsync(Snowflake webhookId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonWebhook>(), this);

    public async Task<Webhook> GetWebhookWithTokenAsync(Snowflake webhookId, string webhookToken, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}/{webhookToken}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonWebhook>(), this);

    public async Task<Webhook> ModifyWebhookAsync(Snowflake webhookId, Action<WebhookOptions> action, RequestProperties? options = null)
    {
        WebhookOptions webhookOptions = new();
        action(webhookOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(webhookOptions), $"/webhooks/{webhookId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonWebhook>(), this);
    }

    public async Task<Webhook> ModifyWebhookAsync(Snowflake webhookId, string webhookToken, Action<WebhookOptions> action, RequestProperties? options = null)
    {
        WebhookOptions webhookOptions = new();
        action(webhookOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(webhookOptions), $"/webhooks/{webhookId}/{webhookToken}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonWebhook>(), this);
    }

    public Task DeleteWebhookAsync(Snowflake webhookId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}", options);

    public Task DeleteWebhookWithTokenAsync(Snowflake webhookId, string webhookToken, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{webhookToken}", options);

    public async Task<RestMessage?> ExecuteWebhookAsync(Snowflake webhookId, string webhookToken, WebhookMessageProperties messageProperties, bool wait = false, Snowflake? threadId = null, RequestProperties? options = null)
    {
        if (wait)
            return new((await SendRequestAsync(HttpMethod.Post, messageProperties.Build(), threadId.HasValue ? $"/webhooks/{webhookId}/{webhookToken}?wait=True&thread_id={threadId.GetValueOrDefault()}" : $"/webhooks/{webhookId}/{webhookToken}?wait=True", options).ConfigureAwait(false)).ToObject<JsonModels.JsonMessage>(), this);
        else
        {
            await SendRequestAsync(HttpMethod.Post, messageProperties.Build(), threadId.HasValue ? $"/webhooks/{webhookId}/{webhookToken}?wait=False&thread_id={threadId.GetValueOrDefault()}" : $"/webhooks/{webhookId}/{webhookToken}?wait=False", options).ConfigureAwait(false);
            return null;
        }
    }

    public async Task<RestMessage> GetWebhookMessageAsync(Snowflake webhookId, string webhookToken, Snowflake messageId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> ModifyWebhookMessageAsync(Snowflake webhookId, string webhookToken, Snowflake messageId, Action<MessageOptions> action, Snowflake? threadId = null, RequestProperties? options = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, messageOptions.Build(), threadId.HasValue ? $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}?thread_id={threadId.GetValueOrDefault()}" : $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonMessage>(), this);
    }

    public Task DeleteWebhookMessageAsync(Snowflake webhookId, string webhookToken, Snowflake messageId, Snowflake? threadId = null, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, threadId.HasValue ? $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}?thread_id={threadId.GetValueOrDefault()}" : $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", options);
}