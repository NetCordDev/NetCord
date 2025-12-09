using NetCord.Services.ApplicationCommands;

namespace Commands;

/// <summary>
/// Examples for the Slash Commands guide.
/// </summary>
public class SlashCommands : ApplicationCommandModule<SlashCommandContext>
{
    // TODO: Add comprehensive slash command examples
    // - Basic slash commands
    // - Commands with options
    // - Subcommands and subcommand groups
    // - Autocomplete
    // - Choices and option types
    
    [SlashCommand("ping", "Check bot latency")]
    public string Ping()
    {
        return "Pong!";
    }
    
    [SlashCommand("echo", "Echo a message")]
    public string Echo([SlashCommandParameter(Description = "Message to echo")] string message)
    {
        return message;
    }
}
