using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("pong")]
    public static string Pong() => "Ping!";
}
