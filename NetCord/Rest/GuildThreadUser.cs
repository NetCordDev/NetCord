using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class GuildThreadUser(JsonThreadUser jsonModel, RestClient client) : ThreadUser(jsonModel, client)
{
    public PartialGuildUser GuildUser { get; } = new(jsonModel.GuildUser!, client);
}
