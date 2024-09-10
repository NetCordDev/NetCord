using NetCord.Services.ApplicationCommands;

namespace MyBot;

public enum Animal
{
    Dog,
    Cat,
    Fish,
    [SlashCommandChoice("Guinea Pig")]
    GuineaPig,
}
