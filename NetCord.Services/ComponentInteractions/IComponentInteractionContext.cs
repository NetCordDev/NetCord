namespace NetCord.Services.ComponentInteractions;

/// <summary>
/// Provides context for handling component interactions such as buttons and select menus.
/// </summary>
public interface IComponentInteractionContext : IInteractionContext
{
    /// <inheritdoc cref="IInteractionContext.Interaction" />
    public new ComponentInteraction Interaction { get; }

    Interaction IInteractionContext.Interaction => Interaction;
}
