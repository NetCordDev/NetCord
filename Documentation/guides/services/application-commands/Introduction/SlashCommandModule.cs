using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class SlashCommandModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("pong", "Pong!")]
    public static string Pong() => "Ping!";
}
