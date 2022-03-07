namespace NetCord;

public class ThreadMemberUpdateEventArgs
{
    public ThreadMemberUpdateEventArgs(ThreadUser user, DiscordId guildId)
    {
        User = user;
        GuildId = guildId;
    }

    public ThreadUser User { get; }

    public DiscordId GuildId { get; }
}
