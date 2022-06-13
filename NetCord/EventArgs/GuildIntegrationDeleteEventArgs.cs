using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildIntegrationDeleteEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs>
{
    JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs _jsonModel;

    public GuildIntegrationDeleteEventArgs(JsonGuildIntegrationDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake IntegrationId => _jsonModel.IntegrationId;

    public Snowflake GuildId => _jsonModel.GuildId;

    public Snowflake? ApplicationId => _jsonModel.ApplicationId;
}
