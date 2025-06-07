using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

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

    [EntryPointCommand("Launch", "Launch!")]
    public static InteractionCallback Launch()
    {
        return InteractionCallback.LaunchActivity;
    }
}

public class DITestModule([FromKeyedServices("key")][Optional][DefaultParameterValue(null)] string? keyedWzium, string wzium) : ApplicationCommandModule<ApplicationCommandContext>
{
    [UserCommand("Wzium")]
    public string Wzium() => $"{keyedWzium} {wzium}";
}
