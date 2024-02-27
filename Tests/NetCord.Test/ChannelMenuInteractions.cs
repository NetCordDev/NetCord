using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class ChannelMenuInteractions : ComponentInteractionModule<ChannelMenuInteractionContext>
{
    [ComponentInteraction("channels")]
    public Task ChannelsAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedChannels)}"));
    }
}
