using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class UserMenuInteraction : EntityMenuInteraction
{
    public UserMenuInteraction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(jsonModel, guild, channel, client)
    {
    }
}
