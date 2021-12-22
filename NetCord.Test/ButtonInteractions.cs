using NetCord.Interactions;

namespace NetCord.Test;

public class ButtonInteractions : ButtonInteractionModule
{
    [Interaction("click it")]
    public Task ClickIt()
    {
        InteractionMessage interactionMessage = new()
        {
            Content = "You clicked the button!",
            Ephemeral = true
        };
        return Context.Interaction.EndWithReplyAsync(interactionMessage);
    }
}