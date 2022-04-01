using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildInviteDeleteEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs _jsonEntity;

    internal GuildInviteDeleteEventArgs(JsonGuildInviteDeleteEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public Snowflake InviteChannelId => _jsonEntity.InviteChannelId;

    public Snowflake? GuildId => _jsonEntity.GuildId;

    public string InviteCode => _jsonEntity.InviteCode;
}
