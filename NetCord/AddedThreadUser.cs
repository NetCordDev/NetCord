using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class AddedThreadUser : ThreadUser
{
    public AddedThreadUser(JsonThreadUser jsonModel, Snowflake guildId, RestClient client) : base(jsonModel, client)
    {
        GuildUser = new(jsonModel.GuildUser!, guildId, client);
        if (jsonModel.Presence != null)
            Presence = new(jsonModel.Presence, guildId, client);
    }

    public GuildUser GuildUser { get; }

    public Presence? Presence { get; }
}
