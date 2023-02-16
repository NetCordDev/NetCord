using System.Runtime.InteropServices;

using NetCord.Rest;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class ButtonInteractions : InteractionModule<ButtonInteractionContext>
{
    [Interaction("click it")]
    public Task ClickIt()
    {
        InteractionMessageProperties interactionMessage = new()
        {
            Content = "You clicked the button!",
            Flags = MessageFlags.Ephemeral
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(interactionMessage));
    }

    [Interaction("test")]
    public Task TestAsync([DefaultParameterValue(null)] params string[]? s)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"values: {(s != null ? string.Join('\n', s) : "null")}"));
    }
}
