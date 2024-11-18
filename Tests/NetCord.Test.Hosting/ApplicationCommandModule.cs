using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

public class ApplicationCommandModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("ping2", "Ping!")]
    public static string Ping()
    {
        return "Pong!";
    }
}
