using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class RoleMenuInteraction : EntityMenuInteraction
{
    public RoleMenuInteraction(JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
    }
}
