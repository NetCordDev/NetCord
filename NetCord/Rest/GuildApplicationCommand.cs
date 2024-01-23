namespace NetCord.Rest;

public partial class GuildApplicationCommand : ApplicationCommand
{
    public ulong GuildId => _jsonModel.GuildId.GetValueOrDefault();

    public GuildApplicationCommand(JsonModels.JsonApplicationCommand jsonModel, RestClient client) : base(jsonModel, client)
    {
    }
}
