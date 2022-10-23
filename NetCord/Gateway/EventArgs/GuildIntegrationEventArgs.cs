using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildIntegrationEventArgs
{
    public GuildIntegrationEventArgs(Integration integration, ulong guildId)
    {
        Integration = integration;
        GuildId = guildId;
    }

    public Integration Integration { get; }

    public ulong GuildId { get; }
}
