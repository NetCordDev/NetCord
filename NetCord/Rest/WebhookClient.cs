namespace NetCord.Rest;

/// <summary>
/// A specialized client that can be used to interact with webhook endpoints.
/// </summary>
public sealed partial class WebhookClient : IDisposable
{
    /// <summary>
    /// The ID of the webhook to use.
    /// </summary>
    public ulong Id { get; }

    /// <summary>
    /// The token to use for authorization.
    /// </summary>
    public string Token { get; }

    private readonly RestClient _client;
    private readonly bool _dispose;

    /// <summary>
    /// Creates a <see cref="WebhookClient"/> from its ID, token, and optional configuration.
    /// </summary>
    /// <param name="webhookId"><inheritdoc cref="Id" path="/summary"/></param>
    /// <param name="webhookToken"><inheritdoc cref="Token" path="/summary"/></param>
    /// <param name="configuration">The configuration to pass to the client.</param>
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
