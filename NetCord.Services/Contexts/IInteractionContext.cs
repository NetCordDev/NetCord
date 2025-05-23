namespace NetCord.Services;

public interface IInteractionContext : IContext
{
    public Interaction Interaction { get; }
}
