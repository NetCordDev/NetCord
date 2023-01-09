using NetCord.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class GuildIntegrationDeleteEventArgs : IJsonModel<JsonGuildIntegrationDeleteEventArgs>
{
    JsonGuildIntegrationDeleteEventArgs IJsonModel<JsonGuildIntegrationDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonGuildIntegrationDeleteEventArgs _jsonModel;

    public GuildIntegrationDeleteEventArgs(JsonGuildIntegrationDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong IntegrationId => _jsonModel.IntegrationId;

    public ulong GuildId => _jsonModel.GuildId;

    public ulong? ApplicationId => _jsonModel.ApplicationId;
}
