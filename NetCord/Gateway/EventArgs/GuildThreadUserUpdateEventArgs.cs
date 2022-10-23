namespace NetCord.Gateway;

public class GuildThreadUserUpdateEventArgs
{
    public GuildThreadUserUpdateEventArgs(ThreadUser user, ulong guildId)
    {
        User = user;
        GuildId = guildId;
    }

    public ThreadUser User { get; }

    public ulong GuildId { get; }
}
