namespace NetCord;

public class GuildDeleteEventArgs
{
    public GuildDeleteEventArgs(DiscordId guildId, bool isUserDeleted)
    {
        GuildId = guildId;
        IsUserDeleted = isUserDeleted;
    }

    public DiscordId GuildId { get; }

    public bool IsUserDeleted { get; }
}
