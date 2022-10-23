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

    public ulong IntegrationId => _jsonModel.IntegrationId;

    public ulong GuildId => _jsonModel.GuildId;

    public ulong? ApplicationId => _jsonModel.ApplicationId;
}
