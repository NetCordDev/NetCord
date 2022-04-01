using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildIntegrationDeleteEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs _jsonEntity;

    internal GuildIntegrationDeleteEventArgs(JsonGuildIntegrationDeleteEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public Snowflake IntegrationId => _jsonEntity.IntegrationId;

    public Snowflake GuildId => _jsonEntity.GuildId;

    public Snowflake? ApplicationId => _jsonEntity.ApplicationId;
}
