using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class RoleMenuModule : InteractionModule<RoleMenuInteractionContext>
{
    [Interaction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedRoles)}"));
    }
}
