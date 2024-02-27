using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class RoleMenuInteractions : ComponentInteractionModule<RoleMenuInteractionContext>
{
    [ComponentInteraction("roles")]
    public Task RolesAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedRoles)}"));
    }
}
