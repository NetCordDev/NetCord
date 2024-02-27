using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ChannelMenuModule : ComponentInteractionModule<ChannelMenuInteractionContext>
{
    [ComponentInteraction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedChannels)}"));
    }
}
