using NetCord.Rest;

namespace NetCord.Gateway;

public class UserMenuInteraction : EntityMenuInteraction
{
    public UserMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
    }
}
