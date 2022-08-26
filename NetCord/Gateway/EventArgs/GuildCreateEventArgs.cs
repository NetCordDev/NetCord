namespace NetCord.Gateway;

public class GuildCreateEventArgs
{
    public GuildCreateEventArgs(Snowflake guildId, Guild? guild)
    {
        GuildId = guildId;
        Guild = guild;
    }

    public Snowflake GuildId { get; }

    public Guild? Guild { get; }
}
