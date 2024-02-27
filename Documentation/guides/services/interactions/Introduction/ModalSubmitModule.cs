using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ModalSubmitModule : ComponentInteractionModule<ModalSubmitInteractionContext>
{
    [ComponentInteraction("modal")]
    public Task ModalAsync()
    {
        return RespondAsync(InteractionCallback.Message(string.Join('\n', Context.Components.Select(c => $"{c.CustomId}: {c.Value}"))));
    }
}
