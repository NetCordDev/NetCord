namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Context for handling autocomplete interactions for application command parameters.
/// </summary>
public interface IAutocompleteInteractionContext : IInteractionContext
{
    /// <inheritdoc cref="IInteractionContext.Interaction" />
    public new AutocompleteInteraction Interaction { get; }

    Interaction IInteractionContext.Interaction => Interaction;
}
