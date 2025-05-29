using NetCord.Services.Commands;

namespace NetCord.Template.Bot.TextCommands;

/// <summary>
/// Example Text command module.
/// To learn more about text commands, check out <see href="https://netcord.dev/guides/services/text-commands/introduction.html?tabs=generic-host">docs</see>.
/// </summary>
/// <param name="logger">Logger injected via DI</param>
public class HelloTextModule(ILogger<HelloTextModule> logger) : CommandModule<CommandContext>
{
    [Command("hello")]
    public string Hello()
    {
        logger.LogInformation("Received hello command from {User}", Context.User.Username);
        return "Hello from NetCord text commands!";
    }
}
