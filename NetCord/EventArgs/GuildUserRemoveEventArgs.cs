namespace NetCord;

public class GuildUserRemoveEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildUserRemoveEventArgs _jsonEntity;

    internal GuildUserRemoveEventArgs(JsonModels.EventArgs.JsonGuildUserRemoveEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        User = new(jsonEntity.User, client);
    }

    public Snowflake GuildId => _jsonEntity.GuildId;

    public User User { get; }
}
