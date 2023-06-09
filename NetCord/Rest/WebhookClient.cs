namespace NetCord.Rest;

public class WebhookClient : IDisposable
{
    public ulong Id { get; }

    private readonly string _token;
    private readonly RestClient _client;
    private readonly bool _dispose;

    public WebhookClient(ulong webhookId, string webhookToken, WebhookClientConfiguration? configuration = null)
    {
        Id = webhookId;
        _token = webhookToken;
        if (configuration is null || configuration.Client is null)
        {
            _dispose = true;
            _client = new();
        }
        else
            _client = configuration.Client;
    }

    public Task<Webhook> GetAsync(RequestProperties? properties = null) => _client.GetWebhookWithTokenAsync(Id, _token, properties);
    public Task<Webhook> ModifyAsync(Action<WebhookOptions> action, RequestProperties? properties = null) => _client.ModifyWebhookWithTokenAsync(Id, _token, action, properties);
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteWebhookWithTokenAsync(Id, _token, properties);
    public Task<RestMessage?> ExecuteAsync(WebhookMessageProperties messageProperties, bool wait = false, ulong? threadId = null, RequestProperties? properties = null) => _client.ExecuteWebhookAsync(Id, _token, messageProperties, wait, threadId, properties);
    public Task<RestMessage> GetMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.GetWebhookMessageAsync(Id, _token, messageId, properties);
    public Task<RestMessage> ModifyMessageAsync(ulong messageId, Action<MessageOptions> action, ulong? threadId = null, RequestProperties? properties = null) => _client.ModifyWebhookMessageAsync(Id, _token, messageId, action, threadId, properties);
    public Task DeleteMessageAsync(ulong messageId, ulong? threadId = null, RequestProperties? properties = null) => _client.DeleteWebhookMessageAsync(Id, _token, messageId, threadId, properties);

    public void Dispose()
    {
        if (_dispose)
            _client.Dispose();
    }
}
