namespace NetCord.Gateway;

public class ThreadUserUpdateEventArgs
{
    public ThreadUserUpdateEventArgs(ThreadUser user, Snowflake guildId)
    {
        User = user;
        GuildId = guildId;
    }

    public ThreadUser User { get; }

    public Snowflake GuildId { get; }
}
