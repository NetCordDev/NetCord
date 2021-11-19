using NetCord.Interactions;

namespace NetCord.Test;

public class ButtonInteractions : ButtonInteractionModule
{
    [Interaction("click it")]
    public async Task ClickIt()
    {
        InteractionMessageBuilder interactionMessage = new()
        {
            Content = "You clicked the button!",
            Ephemeral = true,
        };
        await Context.Interaction.EndWithReplyAsync(interactionMessage.Build());
    }
}