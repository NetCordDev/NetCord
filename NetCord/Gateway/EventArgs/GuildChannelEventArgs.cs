namespace NetCord.Gateway;

public class GuildChannelEventArgs
{
    public GuildChannelEventArgs(IGuildChannel channel, ulong guildId)
    {
        Channel = channel;
        GuildId = guildId;
    }

    public IGuildChannel Channel { get; }

    public ulong GuildId { get; }
}
