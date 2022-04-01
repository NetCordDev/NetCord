namespace NetCord;

public class FollowedChannel : Entity
{
    private readonly JsonModels.JsonFollowedChannel _jsonEntity;

    public override Snowflake Id => _jsonEntity.Id;

    public Snowflake WebhookId => _jsonEntity.WebhookId;

    internal FollowedChannel(JsonModels.JsonFollowedChannel jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}