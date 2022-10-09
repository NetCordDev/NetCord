using NetCord.Services;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class ModalSubmitInteractions : InteractionModule<ModalSubmitInteractionContext>
{
    [Interaction("wzium")]
    public Task WziumAsync(UserId user)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"{user} got wziummed with reason: {Context.Components[0].Value}"));
    }
}
