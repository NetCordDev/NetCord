using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class HelloModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("hello", "Say hello!")]
    public static string Hello([MustContain<SlashCommandContext>("hello")] string text) => text;
}
