using NetCord.Rest;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class ButtonInteractions(string wzium) : InteractionModule<ButtonInteractionContext>
{
    [Interaction("click it")]
    public Task ClickIt()
    {
        //InteractionMessageProperties interactionMessage = new()
        //{
        //    Content = "You clicked the button!",
        //    Flags = MessageFlags.Ephemeral
        //};
        //return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(interactionMessage));

        return RespondAsync(InteractionCallback.Modal(new($"wzium:{Context.User.Id}", $"Wzium user {wzium}",
        [
            new("reason", TextInputStyle.Paragraph, "Reason")
            {
                Placeholder = "Because of not wziumming",
                Required = false,
                MinLength = 5,
                MaxLength = 20,
            },
        ])));
    }

    [Interaction("test")]
    public Task TestAsync(string s, string s2)
    {
        return RespondAsync(InteractionCallback.Message($"{s}\n{s2}"));
    }

    [Interaction("enum")]
    public Task EnumAsync(PaginationDirection paginationDirection)
    {
        return RespondAsync(InteractionCallback.Message(paginationDirection.ToString()));
    }
}
