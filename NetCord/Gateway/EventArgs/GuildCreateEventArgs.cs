namespace NetCord.Gateway;

public class GuildCreateEventArgs
{
    public GuildCreateEventArgs(ulong guildId, Guild? guild)
    {
        GuildId = guildId;
        Guild = guild;
    }

    public ulong GuildId { get; }

    public Guild? Guild { get; }
}
