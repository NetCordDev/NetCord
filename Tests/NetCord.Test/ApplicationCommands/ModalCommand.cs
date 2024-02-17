using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.SlashCommands;

public class ModalCommand : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("wzium", "Wziums user")]
    public Task WziumAsync(User user)
    {
        return RespondAsync(InteractionCallback.Modal(new($"wzium:{user.Id}", "Wzium user",
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
}
