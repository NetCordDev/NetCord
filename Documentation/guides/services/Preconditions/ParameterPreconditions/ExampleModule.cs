using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("hello")]
    public Task HelloAsync([CommandParameter(Remainder = true)][MustContain<CommandContext>("hello")] string text)
    {
        return ReplyAsync(text);
    }
}
