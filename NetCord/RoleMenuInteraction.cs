using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class RoleMenuInteraction : EntityMenuInteraction
{
    public RoleMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, RestClient client) : base(jsonModel, guild, client)
    {
    }
}
