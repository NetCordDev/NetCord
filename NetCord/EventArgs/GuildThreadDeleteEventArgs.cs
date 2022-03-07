namespace NetCord;

public class GuildThreadDeleteEventArgs
{
    internal GuildThreadDeleteEventArgs(DiscordId threadId, DiscordId guildId, DiscordId threadParentId, ChannelType threadType)
    {
        ThreadId = threadId;
        GuildId = guildId;
        ThreadParentId = threadParentId;
        ThreadType = threadType;
    }

    public DiscordId ThreadId { get; }

    public DiscordId GuildId { get; }

    public DiscordId ThreadParentId { get; }

    public ChannelType ThreadType { get; }
}