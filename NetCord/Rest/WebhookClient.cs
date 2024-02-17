namespace NetCord.Rest;

public sealed partial class WebhookClient : IDisposable
{
    public ulong Id { get; }

    public string Token { get; }

    private readonly RestClient _client;
    private readonly bool _dispose;

    public WebhookClient(ulong webhookId, string webhookToken, WebhookClientConfiguration? configuration = null)
    {
        Id = webhookId;
        Token = webhookToken;
        if (configuration is { Client: RestClient client })
            _client = client;
        else
        {
            _dispose = true;
            _client = new();
        }
    }

    public void Dispose()
    {
        if (_dispose)
            _client.Dispose();
    }
}
