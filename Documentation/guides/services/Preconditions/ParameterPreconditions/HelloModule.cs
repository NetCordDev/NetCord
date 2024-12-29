using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class HelloModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("hello", "Say hello!")]
    public static string Hello([MustContain<ApplicationCommandContext>("hello")] string text) => text;
}
