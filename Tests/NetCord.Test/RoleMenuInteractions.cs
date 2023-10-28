using NetCord.Rest;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class RoleMenuInteractions : InteractionModule<RoleMenuInteractionContext>
{
    [Interaction("roles")]
    public Task RolesAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedRoles)}"));
    }
}
