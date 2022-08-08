namespace NetCord.Services.Commands;

public class CommandNotFoundException : Exception
{
    internal CommandNotFoundException() : base("Command not found.")
    {
    }
}