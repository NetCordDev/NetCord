using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<Webhook> CreateWebhookAsync(ulong channelId, WebhookProperties webhookProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<WebhookProperties>(webhookProperties, WebhookProperties.WebhookPropertiesSerializerContext.WithOptions.WebhookProperties))
            return Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/webhooks", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonWebhook.JsonWebhookSerializerContext.WithOptions.JsonWebhook).ConfigureAwait(false), this);
    }

    public async Task<IReadOnlyDictionary<ulong, Webhook>> GetChannelWebhooksAsync(ulong channelId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/webhooks", properties).ConfigureAwait(false)).ToObjectAsync(JsonWebhook.JsonWebhookArraySerializerContext.WithOptions.JsonWebhookArray).ConfigureAwait(false)).ToDictionary(w => w.Id, w => Webhook.CreateFromJson(w, this));

    public async Task<IReadOnlyDictionary<ulong, Webhook>> GetGuildWebhooksAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/webhooks", properties).ConfigureAwait(false)).ToObjectAsync(JsonWebhook.JsonWebhookArraySerializerContext.WithOptions.JsonWebhookArray).ConfigureAwait(false)).ToDictionary(w => w.Id, w => Webhook.CreateFromJson(w, this));

    public async Task<Webhook> GetWebhookAsync(ulong webhookId, RequestProperties? properties = null)
        => Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}", properties).ConfigureAwait(false)).ToObjectAsync(JsonWebhook.JsonWebhookSerializerContext.WithOptions.JsonWebhook).ConfigureAwait(false), this);

    public async Task<Webhook> GetWebhookWithTokenAsync(ulong webhookId, string webhookToken, RequestProperties? properties = null)
        => Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}/{webhookToken}", properties).ConfigureAwait(false)).ToObjectAsync(JsonWebhook.JsonWebhookSerializerContext.WithOptions.JsonWebhook).ConfigureAwait(false), this);

    public async Task<Webhook> ModifyWebhookAsync(ulong webhookId, Action<WebhookOptions> action, RequestProperties? properties = null)
    {
        WebhookOptions webhookOptions = new();
        action(webhookOptions);
        using (HttpContent content = new JsonContent<WebhookOptions>(webhookOptions, WebhookOptions.WebhookOptionsSerializerContext.WithOptions.WebhookOptions))
            return Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, $"/webhooks/{webhookId}", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonWebhook.JsonWebhookSerializerContext.WithOptions.JsonWebhook).ConfigureAwait(false), this);
    }

    public async Task<Webhook> ModifyWebhookWithTokenAsync(ulong webhookId, string webhookToken, Action<WebhookOptions> action, RequestProperties? properties = null)
    {
        WebhookOptions webhookOptions = new();
        action(webhookOptions);
        using (HttpContent content = new JsonContent<WebhookOptions>(webhookOptions, WebhookOptions.WebhookOptionsSerializerContext.WithOptions.WebhookOptions))
            return Webhook.CreateFromJson(await (await SendRequestAsync(HttpMethod.Patch, $"/webhooks/{webhookId}/{webhookToken}", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonWebhook.JsonWebhookSerializerContext.WithOptions.JsonWebhook).ConfigureAwait(false), this);
    }

    public Task DeleteWebhookAsync(ulong webhookId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}", properties);

    public Task DeleteWebhookWithTokenAsync(ulong webhookId, string webhookToken, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{webhookToken}", properties);

    public async Task<RestMessage?> ExecuteWebhookAsync(ulong webhookId, string webhookToken, WebhookMessageProperties message, bool wait = false, ulong? threadId = null, RequestProperties? properties = null)
    {
        using (HttpContent content = message.Build())
        {
            if (wait)
                return new(await (await SendRequestAsync(HttpMethod.Post, threadId.HasValue ? $"/webhooks/{webhookId}/{webhookToken}?wait=True&thread_id={threadId.GetValueOrDefault()}" : $"/webhooks/{webhookId}/{webhookToken}?wait=True", new(RateLimits.RouteParameter.ExecuteWebhookModifyDeleteWebhookMessage), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
            else
            {
                await SendRequestAsync(HttpMethod.Post, threadId.HasValue ? $"/webhooks/{webhookId}/{webhookToken}?wait=False&thread_id={threadId.GetValueOrDefault()}" : $"/webhooks/{webhookId}/{webhookToken}?wait=False", new(RateLimits.RouteParameter.ExecuteWebhookModifyDeleteWebhookMessage), content, properties).ConfigureAwait(false);
                return null;
            }
        }
    }

    public async Task<RestMessage> GetWebhookMessageAsync(ulong webhookId, string webhookToken, ulong messageId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", new RateLimits.Route(RateLimits.RouteParameter.ExecuteWebhookModifyDeleteWebhookMessage), properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);

    public async Task<RestMessage> ModifyWebhookMessageAsync(ulong webhookId, string webhookToken, ulong messageId, Action<MessageOptions> action, ulong? threadId = null, RequestProperties? properties = null)
    {
        MessageOptions messageOptions = new();
        action(messageOptions);
        using (HttpContent content = messageOptions.Build())
            return new(await (await SendRequestAsync(HttpMethod.Patch, threadId.HasValue ? $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}?thread_id={threadId.GetValueOrDefault()}" : $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", new(RateLimits.RouteParameter.ExecuteWebhookModifyDeleteWebhookMessage), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonMessage.JsonMessageSerializerContext.WithOptions.JsonMessage).ConfigureAwait(false), this);
    }

    public Task DeleteWebhookMessageAsync(ulong webhookId, string webhookToken, ulong messageId, ulong? threadId = null, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, threadId.HasValue ? $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}?thread_id={threadId.GetValueOrDefault()}" : $"/webhooks/{webhookId}/{webhookToken}/messages/{messageId}", new RateLimits.Route(RateLimits.RouteParameter.ExecuteWebhookModifyDeleteWebhookMessage), properties);
}
