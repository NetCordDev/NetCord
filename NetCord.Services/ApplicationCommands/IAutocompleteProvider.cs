using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IAutocompleteProvider<TAutocompleteContext> where TAutocompleteContext : IAutocompleteInteractionContext
{
    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, TAutocompleteContext context);
}
