using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class FirstModule : InteractionModule<UserMenuInteractionContext>
{
    [Interaction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"You selected: {string.Join(", ", Context.SelectedUsers)}"));
    }
}
