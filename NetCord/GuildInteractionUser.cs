using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class GuildInteractionUser(JsonGuildUser jsonModel, ulong guildId, RestClient client) : GuildUser(jsonModel, guildId, client)
{
    public Permissions Permissions => jsonModel.Permissions.GetValueOrDefault();
}
