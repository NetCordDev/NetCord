using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class ExampleAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
{
    private readonly string[] _data;

    public ExampleAutocompleteProvider(string[] data)
    {
        _data = data;
    }

    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        var input = option.Value!;
        var result = _data.Where(d => d.Contains(input))
                          .Take(25)
                          .Select(d => new ApplicationCommandOptionChoiceProperties(d, d));

        return new(result);
    }
}
