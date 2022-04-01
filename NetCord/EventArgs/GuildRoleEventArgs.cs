namespace NetCord;

public class GuildRoleEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildRoleEventArgs _jsonEntity;

    internal GuildRoleEventArgs(JsonModels.EventArgs.JsonGuildRoleEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        Role = new(jsonEntity.Role, client);
    }

    public Snowflake GuildId => _jsonEntity.GuildId;

    public GuildRole Role { get; }
}
