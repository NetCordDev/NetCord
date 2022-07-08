namespace NetCord.Gateway;

public class ThreadMemberUpdateEventArgs
{
    public ThreadMemberUpdateEventArgs(ThreadUser user, Snowflake guildId)
    {
        User = user;
        GuildId = guildId;
    }

    public ThreadUser User { get; }

    public Snowflake GuildId { get; }
}