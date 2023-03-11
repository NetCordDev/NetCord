using NetCord.Gateway;

namespace NetCord.Services.ApplicationCommands;

public interface IAutocompleteInteractionContext : IContext
{
    public ApplicationCommandAutocompleteInteraction Interaction { get; }
}
