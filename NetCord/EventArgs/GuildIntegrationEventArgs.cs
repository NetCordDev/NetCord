namespace NetCord;

public class GuildIntegrationEventArgs
{
    public GuildIntegrationEventArgs(Integration integration, DiscordId guildId)
    {
        Integration = integration;
        GuildId = guildId;
    }

    public Integration Integration { get; }

    public DiscordId GuildId { get; }
}
