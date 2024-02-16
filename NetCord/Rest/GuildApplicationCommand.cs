namespace NetCord.Rest;

public partial class GuildApplicationCommand(JsonModels.JsonApplicationCommand jsonModel, RestClient client) : ApplicationCommand(jsonModel, client)
{
    public ulong GuildId => _jsonModel.GuildId.GetValueOrDefault();
}
