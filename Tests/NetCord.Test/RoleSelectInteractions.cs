using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class RoleSelectInteractions : ComponentInteractionModule<RoleSelectInteractionContext>
{
    [ComponentInteraction("roles")]
    public Task RolesAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedValues)}"));
    }
}
