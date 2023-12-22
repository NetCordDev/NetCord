namespace NetCord.Services.Commands;

public class CommandNotFoundException : Exception
{
    public CommandNotFoundException() : base("Command not found.")
    {
    }
}
