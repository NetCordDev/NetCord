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
            new LabelProperties(
                "Reason",
                new TextInputProperties("reason", TextInputStyle.Paragraph)
                {
                    Placeholder = "Because of not wziumming",
                    Required = false,
                    MinLength = 5,
                    MaxLength = 20,
                    Id = 1223,
                }
            ).WithId(102),
            new LabelProperties(
                "XD",
                new TextInputProperties("wzium", TextInputStyle.Short)
                {
                    Placeholder = "Wzium or not",
                    Required = true,
                    Id = 5,
                }
            ).WithId(24),
            new LabelProperties(
                "Menu",
                new StringMenuProperties("xd")
                {
                    new StringMenuSelectOptionProperties("Yes", "yes"),
                    new StringMenuSelectOptionProperties("No", "no"),
                }.WithId(12).WithRequired(true)
            ).WithId(13),
        ])));
    }
}
