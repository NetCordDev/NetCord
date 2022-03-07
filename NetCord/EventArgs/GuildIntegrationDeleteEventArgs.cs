using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildIntegrationDeleteEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs _jsonEntity;

    internal GuildIntegrationDeleteEventArgs(JsonGuildIntegrationDeleteEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public DiscordId IntegrationId => _jsonEntity.IntegrationId;

    public DiscordId GuildId => _jsonEntity.GuildId;

    public DiscordId? ApplicationId => _jsonEntity.ApplicationId;
}
