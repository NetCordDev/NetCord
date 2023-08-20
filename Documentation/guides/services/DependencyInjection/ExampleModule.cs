using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    private readonly string _botName;

    public ExampleModule(string botName)
    {
        _botName = botName;
    }

    [Command("name")]
    public Task NameAsync()
    {
        return ReplyAsync(_botName);
    }
}
