using NetCord.Services.Interactions;

namespace NetCord.Test;

public class ButtonInteractions : BaseInteractionModule<ButtonInteractionContext>
{
    [Interaction("click it")]
    public Task ClickIt()
    {
        InteractionMessage interactionMessage = new()
        {
            Content = "You clicked the button!",
            Flags = MessageFlags.Ephemeral
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(interactionMessage));
    }
}