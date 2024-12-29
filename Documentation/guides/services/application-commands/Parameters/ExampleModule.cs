using NetCord;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class ExampleModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("username", "Returns user's username")]
    public string Username(User? user = null)
    {
        user ??= Context.User;
        return user.Username;
    }

    [SlashCommand("power", "Raises a number to a power")]
    public static string Power(
        [SlashCommandParameter(Name = "base", Description = "The base")] double @base,
        [SlashCommandParameter(Description = "The power")] double power = 2)
    {
        return $"Result: {Math.Pow(@base, power)}";
    }

    [SlashCommand("animal", "Sends animal you selected")]
    public static string Animal(Animal animal) => animal.ToString();
}
