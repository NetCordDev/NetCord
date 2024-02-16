namespace NetCord.Gateway;

public class GuildThreadUserUpdateEventArgs(ThreadUser user, ulong guildId)
{
    public ThreadUser User { get; } = user;

    public ulong GuildId { get; } = guildId;
}
