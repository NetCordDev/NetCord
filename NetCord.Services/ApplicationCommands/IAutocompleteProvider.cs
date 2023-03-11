using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IAutocompleteProvider<TContext> : IAutocompleteProvider where TContext : IAutocompleteInteractionContext
{
    public Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(TContext context, ApplicationCommandInteractionDataOption option);
}

public interface IAutocompleteProvider
{
}
