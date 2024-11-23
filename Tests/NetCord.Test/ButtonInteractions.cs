using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class ButtonInteractions(string wzium) : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("click it")]
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
            new TextInputProperties("reason", TextInputStyle.Paragraph, "Reason")
            {
                Placeholder = "Because of not wziumming",
                Required = false,
                MinLength = 5,
                MaxLength = 20,
            },
        ])));
    }

    [ComponentInteraction("test")]
    public Task TestAsync(string s, string s2)
    {
        return RespondAsync(InteractionCallback.Message($"{s}\n{s2}"));
    }

    [ComponentInteraction("enum")]
    public Task EnumAsync(PaginationDirection paginationDirection)
    {
        return RespondAsync(InteractionCallback.Message(paginationDirection.ToString()));
    }
}
