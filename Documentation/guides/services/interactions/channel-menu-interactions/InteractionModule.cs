using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<ChannelMenuInteractionContext>
{
    [Interaction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"You selected: {string.Join(", ", Context.SelectedChannels.Values)}"));
    }
}
