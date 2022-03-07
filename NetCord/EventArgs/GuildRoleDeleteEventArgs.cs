using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildRoleDeleteEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildRoleDeleteEventArgs _jsonEntity;

    internal GuildRoleDeleteEventArgs(JsonGuildRoleDeleteEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public DiscordId GuildId => _jsonEntity.GuildId;

    public DiscordId RoleId => _jsonEntity.RoleId;
}