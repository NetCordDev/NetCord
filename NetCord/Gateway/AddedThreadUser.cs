using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class AddedThreadUser : ThreadUser
{
    public AddedThreadUser(JsonThreadUser jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildUser = new(jsonModel.GuildUser!, guildId, client);
        if (jsonModel.Presence != null)
            Presence = new(jsonModel.Presence, guildId, client);
    }

    public GuildUser GuildUser { get; }

    public Presence? Presence { get; }
}
