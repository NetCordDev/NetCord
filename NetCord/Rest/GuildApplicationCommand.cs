using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class GuildApplicationCommand(JsonApplicationCommand jsonModel, RestClient client) : ApplicationCommand(jsonModel, client)
{
    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();
}
