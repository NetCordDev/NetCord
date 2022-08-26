namespace NetCord.Gateway;

public class GuildThreadEventArgs
{
    internal GuildThreadEventArgs(GuildThread thread, Snowflake guildId)
    {
        Thread = thread;
        GuildId = guildId;
    }

    public GuildThread Thread { get; }

    public Snowflake GuildId { get; }
}
