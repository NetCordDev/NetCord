using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule(string botName) : CommandModule<CommandContext>
{
    [Command("name")]
    public string Name() => botName;
}
