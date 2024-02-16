using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class IncomingWebhook(JsonWebhook jsonModel, RestClient client) : Webhook(jsonModel, client)
{
    public string Token => _jsonModel.Token!;
}
