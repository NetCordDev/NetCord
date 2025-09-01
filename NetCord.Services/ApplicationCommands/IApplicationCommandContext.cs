namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Context for handling application command interactions.
/// </summary>
public interface IApplicationCommandContext : IInteractionContext
{
    /// <inheritdoc cref="IInteractionContext.Interaction" />
    public new ApplicationCommandInteraction Interaction { get; }

    Interaction IInteractionContext.Interaction => Interaction;
}
