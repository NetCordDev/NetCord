namespace NetCord;

public class GuildIntegrationsUpdateEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs _jsonEntity;

    internal GuildIntegrationsUpdateEventArgs(JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public Snowflake GuildId => _jsonEntity.GuildId;
}
