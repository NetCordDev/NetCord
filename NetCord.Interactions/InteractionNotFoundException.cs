namespace NetCord.Interactions;

public class InteractionNotFoundException : Exception
{
    internal InteractionNotFoundException() : base("Interaction not found")
    {
    }
}