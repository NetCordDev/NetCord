namespace NetCord.Commands;

public class InvalidCommandDefinitionException : Exception
{
    public InvalidCommandDefinitionException(string message) : base(message)
    {
    }
}