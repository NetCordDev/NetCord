namespace NetCord;

public class GuildChannelEventArgs
{
    internal GuildChannelEventArgs(IGuildChannel channel, DiscordId guildId)
    {
        Channel = channel;
        GuildId = guildId;
    }

    public IGuildChannel Channel { get; }

    public DiscordId GuildId { get; }
}
