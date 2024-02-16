namespace NetCord.Gateway;

public class GuildIntegrationEventArgs(Integration integration, ulong guildId)
{
    public Integration Integration { get; } = integration;

    public ulong GuildId { get; } = guildId;
}
