namespace NetCord.Services.ComponentInteractions;

public interface IComponentInteractionContext : IInteractionContext
{
    public new ComponentInteraction Interaction { get; }

    Interaction IInteractionContext.Interaction => Interaction;
}
