using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("hello")]
    public static string Hello([CommandParameter(Remainder = true)][MustContain<CommandContext>("hello")] string text) => text;
}
