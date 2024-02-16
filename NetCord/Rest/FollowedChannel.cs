namespace NetCord.Rest;

public class FollowedChannel(JsonModels.JsonFollowedChannel jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonFollowedChannel>
{
    JsonModels.JsonFollowedChannel IJsonModel<JsonModels.JsonFollowedChannel>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    public ulong WebhookId => jsonModel.WebhookId;
}
