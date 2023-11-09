using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IAutocompleteInfo
{
    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> InvokeAutocompleteAsync<TAutocompleteContext>(TAutocompleteContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, IServiceProvider? serviceProvider) where TAutocompleteContext : IAutocompleteInteractionContext;

    internal void InitializeAutocomplete<TAutocompleteContext>() where TAutocompleteContext : IAutocompleteInteractionContext;
}
