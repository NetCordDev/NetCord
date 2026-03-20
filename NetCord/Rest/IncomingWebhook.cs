using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class IncomingWebhook(JsonWebhook jsonModel, RestClient client) : Webhook(jsonModel, client)
{
    public string Token => _jsonModel.Token!;

    public WebhookClient ToClient(WebhookClientConfiguration? configuration = null)
    {
        if (configuration is { Client: not null })
            return new(Id, Token, configuration);

        return new(Id, Token, new() { Client = _client });
    }
}
