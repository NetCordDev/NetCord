using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class ExampleModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("username", "Returns user's username")]
    public Task UsernameAsync(User? user = null)
    {
        user ??= Context.User;
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(user.Username));
    }

    [SlashCommand("power", "Raises a number to a power")]
    public Task PowerAsync([SlashCommandParameter(Name = "base", Description = "The base")] double @base, [SlashCommandParameter(Description = "The power")] double power = 2)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"Result: {Math.Pow(@base, power)}"));
    }

    [SlashCommand("animal", "Sends animal you selected")]
    public Task AnimalAsync(Animal animal)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(animal.ToString()));
    }

    public enum Animal
    {
        Dog,
        Cat,
        Fish,
        [SlashCommandChoice(Name = "Guinea Pig")]
        GuineaPig,
    }
}
