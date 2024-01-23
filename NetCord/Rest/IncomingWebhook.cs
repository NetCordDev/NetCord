using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class IncomingWebhook : Webhook
{
    public IncomingWebhook(JsonWebhook jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public string Token => _jsonModel.Token!;
}
