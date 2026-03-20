using NetCord.Services.ApplicationCommands;

namespace MyBot;

public enum Animal
{
    Dog,
    Cat,
    Fish,
    [SlashCommandChoice(Name = "Guinea Pig")]
    GuineaPig,
}
