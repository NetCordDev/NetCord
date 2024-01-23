namespace NetCord.Rest;

public partial class WebhookClient : IDisposable
{
    public ulong Id { get; }

    public string Token { get; }

    private readonly RestClient _client;
    private readonly bool _dispose;

    public WebhookClient(ulong webhookId, string webhookToken, WebhookClientConfiguration? configuration = null)
    {
        Id = webhookId;
        Token = webhookToken;
        if (configuration is null || configuration.Client is null)
        {
            _dispose = true;
            _client = new();
        }
        else
            _client = configuration.Client;
    }

    public void Dispose()
    {
        if (_dispose)
            _client.Dispose();
    }
}
