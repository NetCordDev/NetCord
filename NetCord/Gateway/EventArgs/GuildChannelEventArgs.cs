namespace NetCord.Gateway;

public class GuildChannelEventArgs
{
    internal GuildChannelEventArgs(IGuildChannel channel, Snowflake guildId)
    {
        Channel = channel;
        GuildId = guildId;
    }

    public IGuildChannel Channel { get; }

    public Snowflake GuildId { get; }
}
