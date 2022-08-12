using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<ButtonInteractionContext>
{
    [Interaction("button")]
    public Task ButtonAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource("You clicked a button!"));
    }
}