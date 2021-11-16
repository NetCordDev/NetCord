namespace NetCord.Interactions;

internal class InvalidInteractionDefinitionException : Exception
{
    public InvalidInteractionDefinitionException(string message) : base(message)
    {
    }
}