using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("pong")]
    public static string Pong() => "Ping!";
    
    [Command("ping")]
    public string PongWithContext() => $"Pong with {Context.Client.Id}";
}
