using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

public class ModalCommand : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("wzium", "Wziums user")]
    public Task WziumAsync(User user)
    {
        return RespondAsync(InteractionCallback.Modal(new($"wzium:{user.Id}", "Wzium user",
        [
            new TextInputProperties("reason", TextInputStyle.Paragraph, "Reason")
            {
                Placeholder = "Because of not wziumming",
                Required = false,
                MinLength = 5,
                MaxLength = 20,
                Id = 1223,
                ParentId = 102,
            },
            new TextInputProperties("wzium", TextInputStyle.Short, "XD")
            {
                Placeholder = "Wzium or not",
                Required = true,
                Id = 5,
                ParentId = 24,
            },
        ])));
    }
}
