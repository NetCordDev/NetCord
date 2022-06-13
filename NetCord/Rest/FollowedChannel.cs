namespace NetCord;

public class FollowedChannel : Entity, IJsonModel<JsonModels.JsonFollowedChannel>
{
    JsonModels.JsonFollowedChannel IJsonModel<JsonModels.JsonFollowedChannel>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonFollowedChannel _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public Snowflake WebhookId => _jsonModel.WebhookId;

    public FollowedChannel(JsonModels.JsonFollowedChannel jsonModel)
    {
        _jsonModel = jsonModel;
    }
}