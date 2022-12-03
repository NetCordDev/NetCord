using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<RoleMenuInteractionContext>
{
    [Interaction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"You selected: {string.Join(", ", Context.SelectedRoles.Values)}"));
    }
}
