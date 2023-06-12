using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class GuildInteractionUser : GuildUser
{
    public GuildInteractionUser(JsonGuildUser jsonModel, ulong guildId, RestClient client) : base(jsonModel, guildId, client)
    {
    }

    public Permissions Permissions => _jsonModel.Permissions.GetValueOrDefault();
}
