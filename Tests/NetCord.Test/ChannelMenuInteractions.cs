using NetCord.Rest;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class ChannelMenuInteractions : InteractionModule<ChannelMenuInteractionContext>
{
    [Interaction("channels")]
    public Task ChannelsAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedChannels)}"));
    }
}
