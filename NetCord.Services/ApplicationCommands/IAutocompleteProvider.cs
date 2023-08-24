using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IAutocompleteProvider<TAutocompleteContext> where TAutocompleteContext : IAutocompleteInteractionContext
{
    public Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, TAutocompleteContext context);
}
