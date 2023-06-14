using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class AddedThreadUser : ThreadUser
{
    public AddedThreadUser(JsonThreadUser jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildUser = new(jsonModel.GuildUser!, guildId, client);

        var presence = jsonModel.Presence;
        if (presence is not null)
            Presence = new(presence, guildId, client);
    }

    public GuildUser GuildUser { get; }

    public Presence? Presence { get; }
}
