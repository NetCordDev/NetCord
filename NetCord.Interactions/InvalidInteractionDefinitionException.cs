namespace NetCord.Interactions;

public class InvalidInteractionDefinitionException : Exception
{
    internal InvalidInteractionDefinitionException(string message) : base(message)
    {
    }
}