using NetCord.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a newly-created webhook, containing its token and standard fields.
/// </summary>
public partial class IncomingWebhook(JsonWebhook jsonModel, RestClient client) : Webhook(jsonModel, client)
{
    /// <summary>
    /// The token of the newly created webhook.
    /// </summary>
    public string Token => _jsonModel.Token!;

    /// <summary>
    /// Creates a usable <see cref="WebhookClient"/> from the webhook object directly.
    /// </summary>
    /// <param name="configuration">The configuration to pass to the client.</param>
    public WebhookClient ToClient(WebhookClientConfiguration? configuration = null)
    {
        if (configuration is { Client: not null })
            return new(Id, Token, configuration);

        return new(Id, Token, new() { Client = _client });
    }
}
