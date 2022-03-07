using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildInviteDeleteEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs _jsonEntity;

    internal GuildInviteDeleteEventArgs(JsonGuildInviteDeleteEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public DiscordId InviteChannelId => _jsonEntity.InviteChannelId;

    public DiscordId? GuildId => _jsonEntity.GuildId;

    public string InviteCode => _jsonEntity.InviteCode;
}
