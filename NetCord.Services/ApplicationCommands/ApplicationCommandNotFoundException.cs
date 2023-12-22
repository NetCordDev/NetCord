namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandNotFoundException : Exception
{
    public ApplicationCommandNotFoundException() : base("Command not found.")
    {
    }
}
