using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class AnimalModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("animal", "Sends the animal you selected")]
    public static string Animal(
        [SlashCommandParameter(Description = "Animal to send")] Animal animal)
    {
        return animal.ToString();
    }
}

public enum Animal
{
    Dog,
    Cat,
    Fish,
    [SlashCommandChoice("Guinea Pig")]
    GuineaPig,
}
