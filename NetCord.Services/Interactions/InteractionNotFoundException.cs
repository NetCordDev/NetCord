namespace NetCord.Services.Interactions;

public class InteractionNotFoundException : Exception
{
    internal InteractionNotFoundException() : base("Interaction not found.")
    {
    }
}