namespace NetCord.Gateway;

public class GuildIntegrationDeleteEventArgs(JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs>
{
    JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationDeleteEventArgs>.JsonModel => jsonModel;

    public ulong IntegrationId => jsonModel.IntegrationId;

    public ulong GuildId => jsonModel.GuildId;

    public ulong? ApplicationId => jsonModel.ApplicationId;
}
