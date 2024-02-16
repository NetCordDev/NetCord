namespace NetCord.Gateway;

public class GuildIntegrationsUpdateEventArgs(JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs>
{
    JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildIntegrationsUpdateEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;
}
