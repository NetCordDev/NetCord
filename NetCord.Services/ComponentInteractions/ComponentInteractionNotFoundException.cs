namespace NetCord.Services.ComponentInteractions;

public class ComponentInteractionNotFoundException : Exception
{
    public ComponentInteractionNotFoundException() : base("Interaction not found.")
    {
    }
}
