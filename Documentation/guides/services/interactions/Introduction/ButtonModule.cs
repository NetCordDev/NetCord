using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class ButtonModule : InteractionModule<ButtonInteractionContext>
{
    [Interaction("button")]
    public Task ButtonAsync()
    {
        return RespondAsync(InteractionCallback.Message("You clicked a button!"));
    }
}
