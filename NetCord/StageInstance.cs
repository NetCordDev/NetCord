namespace NetCord;

public class StageInstance : ClientEntity
{
    private readonly JsonModels.JsonStageInstance _jsonEntity;

    public override Snowflake Id => _jsonEntity.Id;
    public Snowflake GuildId => _jsonEntity.GuildId;
    public Snowflake ChannelId => _jsonEntity.ChannelId;
    public string Topic => _jsonEntity.Topic;
    public StageInstancePrivacyLevel PrivacyLevel => _jsonEntity.PrivacyLevel;
    public bool DiscoverableDisabled => _jsonEntity.DiscoverableDisabled;

    internal StageInstance(JsonModels.JsonStageInstance jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
    }
}