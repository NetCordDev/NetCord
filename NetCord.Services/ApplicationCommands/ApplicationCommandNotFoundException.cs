namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandNotFoundException : Exception
{
    internal ApplicationCommandNotFoundException() : base("Command not found.")
    {
    }
}
