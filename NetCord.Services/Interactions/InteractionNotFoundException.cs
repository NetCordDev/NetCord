namespace NetCord.Services.Interactions;

public class InteractionNotFoundException : Exception
{
    public InteractionNotFoundException() : base("Interaction not found.")
    {
    }
}
