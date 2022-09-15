namespace NetCord.Gateway;

public class GuildThreadDeleteEventArgs
{
    public GuildThreadDeleteEventArgs(Snowflake threadId, Snowflake guildId, Snowflake threadParentId, ChannelType threadType)
    {
        ThreadId = threadId;
        GuildId = guildId;
        ThreadParentId = threadParentId;
        ThreadType = threadType;
    }

    public Snowflake ThreadId { get; }

    public Snowflake GuildId { get; }

    public Snowflake ThreadParentId { get; }

    public ChannelType ThreadType { get; }
}
