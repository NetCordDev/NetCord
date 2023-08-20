using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class FirstModule : InteractionModule<ModalSubmitInteractionContext>
{
    [Interaction("modal")]
    public Task ModalAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(string.Join('\n', Context.Components.Select(c => $"{c.CustomId}: {c.Value}"))));
    }
}
