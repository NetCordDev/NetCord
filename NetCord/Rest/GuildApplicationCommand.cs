namespace NetCord.Rest;

public partial class GuildApplicationCommand(JsonModels.JsonApplicationCommand jsonModel, RestClient client) : ApplicationCommand(jsonModel, client)
{
    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();
}
