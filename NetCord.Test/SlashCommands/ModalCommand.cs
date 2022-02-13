using NetCord.Services.SlashCommands;

namespace NetCord.Test.SlashCommands;

public class ModalCommand : SlashCommandModule<SlashCommandContext>
{
    [SlashCommand("wzium", "Wziums user")]
    public Task WziumAsync(User user)
    {
        return RespondAsync(InteractionCallback.Modal(new($"wzium:{user.Id}", "Wzium user", new List<TextInputProperties>()
        {
            new("reason", TextInputStyle.Paragraph, "Reason")
            {
                Placeholder = "Because of not wziumming",
            },
        })));
    }
}