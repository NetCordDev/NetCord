using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

public class ApplicationCommandModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("ping2", "Ping!")]
    public static string Ping()
    {
        return "Pong!";
    }

    [UserCommand("Mention")]
    public static string Mention(User user)
    {
        return user.ToString();
    }

    [MessageCommand("Length")]
    public static string Length(RestMessage message)
    {
        return message.Content.Length.ToString();
    }
}
