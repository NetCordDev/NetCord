namespace NetCord.Rest;

public class FollowedChannel : ClientEntity, IJsonModel<JsonModels.JsonFollowedChannel>
{
    JsonModels.JsonFollowedChannel IJsonModel<JsonModels.JsonFollowedChannel>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonFollowedChannel _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public ulong WebhookId => _jsonModel.WebhookId;

    public FollowedChannel(JsonModels.JsonFollowedChannel jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }
}
