namespace NetCord.Gateway;

public class GuildChannelEventArgs
{
    public GuildChannelEventArgs(IGuildChannel channel, Snowflake guildId)
    {
        Channel = channel;
        GuildId = guildId;
    }

    public IGuildChannel Channel { get; }

    public Snowflake GuildId { get; }
}
