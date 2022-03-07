namespace NetCord;

public class GuildThreadEventArgs
{
    internal GuildThreadEventArgs(GuildThread thread, DiscordId guildId)
    {
        Thread = thread;
        GuildId = guildId;
    }

    public GuildThread Thread { get; }

    public DiscordId GuildId { get; }
}
