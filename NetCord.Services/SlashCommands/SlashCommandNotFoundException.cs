namespace NetCord.Services.SlashCommands;

public class SlashCommandNotFoundException : Exception
{
    internal SlashCommandNotFoundException() : base("Command not found")
    {
    }
}