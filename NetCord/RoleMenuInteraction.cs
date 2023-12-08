using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public class RoleMenuInteraction : EntityMenuInteraction
{
    public RoleMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
    }
}
