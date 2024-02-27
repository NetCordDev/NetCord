using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ButtonModule : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("button")]
    public Task ButtonAsync()
    {
        return RespondAsync(InteractionCallback.Message("You clicked a button!"));
    }
}
