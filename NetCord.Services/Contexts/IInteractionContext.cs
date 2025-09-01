namespace NetCord.Services;

/// <summary>
/// Provides access to interaction data.
/// </summary>
public interface IInteractionContext : IContext
{
    /// <summary>
    /// The interaction that is being handled.
    /// </summary>
    public Interaction Interaction { get; }
}
