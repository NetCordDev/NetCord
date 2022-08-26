namespace NetCord.Gateway;

public class GuildDeleteEventArgs
{
    public GuildDeleteEventArgs(Snowflake guildId, bool isUserDeleted)
    {
        GuildId = guildId;
        IsUserDeleted = isUserDeleted;
    }

    public Snowflake GuildId { get; }

    public bool IsUserDeleted { get; }
}
