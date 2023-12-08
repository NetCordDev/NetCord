using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

public class SlashCommandModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("ping2", "Ping!")]
    public static string Ping()
    {
        return "Pong!";
    }
}
