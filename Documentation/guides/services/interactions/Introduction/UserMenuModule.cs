using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class UserMenuModule : ComponentInteractionModule<UserMenuInteractionContext>
{
    [ComponentInteraction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedUsers)}"));
    }
}
