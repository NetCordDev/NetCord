namespace NetCord.Gateway;

public class GuildDeleteEventArgs
{
    public GuildDeleteEventArgs(ulong guildId, bool isUserDeleted)
    {
        GuildId = guildId;
        IsUserDeleted = isUserDeleted;
    }

    public ulong GuildId { get; }

    public bool IsUserDeleted { get; }
}
