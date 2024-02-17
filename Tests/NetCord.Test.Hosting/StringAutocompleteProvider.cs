using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

internal class StringAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
{
    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        return new(
        [
            new("test", "test"),
            new("test2", "test2"),
            new("test3", "test3"),
            new("test4", "test4"),
            new("test5", "test5"),
            new("test6", "test6"),
        ]);
    }
}
