using NetCord.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class IntegrationDeleteEventArgs : IJsonModel<JsonIntegrationDeleteEventArgs>
{
    JsonIntegrationDeleteEventArgs IJsonModel<JsonIntegrationDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonIntegrationDeleteEventArgs _jsonModel;

    public IntegrationDeleteEventArgs(JsonIntegrationDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake IntegrationId => _jsonModel.IntegrationId;

    public Snowflake GuildId => _jsonModel.GuildId;

    public Snowflake? ApplicationId => _jsonModel.ApplicationId;
}
