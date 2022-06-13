namespace NetCord;

public class StageInstance : ClientEntity, IJsonModel<JsonModels.JsonStageInstance>
{
    JsonModels.JsonStageInstance IJsonModel<JsonModels.JsonStageInstance>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonStageInstance _jsonModel;

    public override Snowflake Id => _jsonModel.Id;
    public Snowflake GuildId => _jsonModel.GuildId;
    public Snowflake ChannelId => _jsonModel.ChannelId;
    public string Topic => _jsonModel.Topic;
    public StageInstancePrivacyLevel PrivacyLevel => _jsonModel.PrivacyLevel;
    public bool DiscoverableDisabled => _jsonModel.DiscoverableDisabled;

    public StageInstance(JsonModels.JsonStageInstance jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }
}