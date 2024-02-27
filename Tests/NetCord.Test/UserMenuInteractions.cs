using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class UserMenuInteractions : ComponentInteractionModule<UserMenuInteractionContext>
{
    [ComponentInteraction("users")]
    public Task UsersAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedUsers)}"));
    }
}
