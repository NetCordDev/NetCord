namespace NetCord.Gateway;

public class GuildIntegrationDeleteEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs>
{
    JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs _jsonModel;

    public GuildIntegrationDeleteEventArgs(JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong IntegrationId => _jsonModel.IntegrationId;

    public ulong GuildId => _jsonModel.GuildId;

    public ulong? ApplicationId => _jsonModel.ApplicationId;
}
