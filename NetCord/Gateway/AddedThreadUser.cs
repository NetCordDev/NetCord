using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class AddedThreadUser(JsonThreadUser jsonModel, ulong guildId, RestClient client) : ThreadUser(jsonModel, client)
{
    public GuildUser GuildUser { get; } = new(jsonModel.GuildUser!, guildId, client);

    public Presence? Presence { get; } = jsonModel.Presence is { } presence ? new(presence, guildId, client) : null;
}
