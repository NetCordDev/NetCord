using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildIntegrationEventArgs
{
    public GuildIntegrationEventArgs(Integration integration, Snowflake guildId)
    {
        Integration = integration;
        GuildId = guildId;
    }

    public Integration Integration { get; }

    public Snowflake GuildId { get; }
}
