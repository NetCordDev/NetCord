namespace NetCord.Gateway;

public class GuildIntegrationsUpdateEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs>
{
    JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs _jsonModel;

    public GuildIntegrationsUpdateEventArgs(JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake GuildId => _jsonModel.GuildId;
}
