using NetCord.Services.ApplicationCommands;

namespace SlashCommands;

public class PingModule : ApplicationCommandModule<SlashCommandContext>
{
    // TODO: Add sample slash commands
    [SlashCommand("ping", "Check bot latency")]
    public string Ping()
    {
        return "Pong!";
    }
}
