namespace NetCord.Services.ApplicationCommands;

public interface IAutocompleteInteractionContext : IInteractionContext
{
    public new AutocompleteInteraction Interaction { get; }

    Interaction IInteractionContext.Interaction => Interaction;
}
