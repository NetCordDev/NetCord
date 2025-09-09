using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModuleWithContext : CommandModule<CommandContext>
{
    [Command("pong")]
    public string Pong() => $"Ping from {Context.Client.Id}";
}
