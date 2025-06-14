using NetCord.Hosting.Services;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

internal class ContextAccessorAutocompleteProvider(IContextAccessor<AutocompleteInteractionContext> contextAccessor) : IAutocompleteProvider<AutocompleteInteractionContext>
{
    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        var s = (contextAccessor.Context == context).ToString();
        return new([new(s, s)]);
    }
}
