namespace NetCord;

public class GuildWidgetChannel : Entity
{
    private readonly JsonModels.JsonGuildWidgetChannel _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public string Name => _jsonEntity.Name;

    public int Position => _jsonEntity.Position;

    internal GuildWidgetChannel(JsonModels.JsonGuildWidgetChannel jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}