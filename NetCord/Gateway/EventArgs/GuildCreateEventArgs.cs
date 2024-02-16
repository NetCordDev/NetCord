namespace NetCord.Gateway;

public class GuildCreateEventArgs(ulong guildId, Guild? guild)
{
    public ulong GuildId { get; } = guildId;

    public Guild? Guild { get; } = guild;
}
