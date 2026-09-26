using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("ping")]
    public string Ping() => $"Pong! {Math.Round(Context.Client.Latency.TotalMilliseconds)} ms";
}
