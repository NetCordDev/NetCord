namespace NetCord.Gateway;

public class GuildDeleteEventArgs(ulong guildId, bool isUserDeleted)
{
    public ulong GuildId { get; } = guildId;

    public bool IsUserDeleted { get; } = isUserDeleted;
}
