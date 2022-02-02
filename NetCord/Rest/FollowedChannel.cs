namespace NetCord;

public class FollowedChannel : Entity
{
    private readonly JsonModels.JsonFollowedChannel _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public DiscordId WebhookId => _jsonEntity.WebhookId;

    internal FollowedChannel(JsonModels.JsonFollowedChannel jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}