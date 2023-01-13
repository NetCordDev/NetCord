using NetCord.JsonModels;

namespace NetCord.Rest;

public class GuildThreadUser : ThreadUser
{
    public GuildThreadUser(JsonThreadUser jsonModel, RestClient client) : base(jsonModel, client)
    {
        GuildUser = new(jsonModel.GuildUser!, client);
    }

    public PartialGuildUser GuildUser { get; }
}
