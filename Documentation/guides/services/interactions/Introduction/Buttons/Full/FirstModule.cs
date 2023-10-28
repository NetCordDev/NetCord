using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class FirstModule : InteractionModule<ButtonInteractionContext>
{
    [Interaction("button")]
    public Task ButtonAsync()
    {
        return RespondAsync(InteractionCallback.Message("You clicked a button!"));
    }
}
