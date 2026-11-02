using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class UserSelectInteractions : ComponentInteractionModule<UserSelectInteractionContext>
{
    [ComponentInteraction("users")]
    public Task UsersAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedValues)}"));
    }
}
