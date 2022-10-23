namespace NetCord.Gateway;

public class GuildThreadDeleteEventArgs
{
    public GuildThreadDeleteEventArgs(ulong threadId, ulong guildId, ulong threadParentId, ChannelType threadType)
    {
        ThreadId = threadId;
        GuildId = guildId;
        ThreadParentId = threadParentId;
        ThreadType = threadType;
    }

    public ulong ThreadId { get; }

    public ulong GuildId { get; }

    public ulong ThreadParentId { get; }

    public ChannelType ThreadType { get; }
}
