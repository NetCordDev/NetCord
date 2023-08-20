using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class FirstModule : InteractionModule<RoleMenuInteractionContext>
{
    [Interaction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"You selected: {string.Join(", ", Context.SelectedRoles)}"));
    }
}
