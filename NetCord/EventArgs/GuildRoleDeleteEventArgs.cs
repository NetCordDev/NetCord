using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildRoleDeleteEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildRoleDeleteEventArgs _jsonEntity;

    internal GuildRoleDeleteEventArgs(JsonGuildRoleDeleteEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public Snowflake GuildId => _jsonEntity.GuildId;

    public Snowflake RoleId => _jsonEntity.RoleId;
}