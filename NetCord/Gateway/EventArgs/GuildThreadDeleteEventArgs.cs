namespace NetCord.Gateway;

public class GuildThreadDeleteEventArgs(ulong threadId, ulong guildId, ulong threadParentId, ChannelType threadType)
{
    public ulong ThreadId { get; } = threadId;

    public ulong GuildId { get; } = guildId;

    public ulong ThreadParentId { get; } = threadParentId;

    public ChannelType ThreadType { get; } = threadType;
}
