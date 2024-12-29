using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class DataAutocompleteProvider(ISomeService someService) : IAutocompleteProvider<AutocompleteInteractionContext>
{
    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(
        ApplicationCommandInteractionDataOption option,
        AutocompleteInteractionContext context)
    {
        var input = option.Value!;
        var data = someService.GetSomeData();

        var result = data.Where(d => d.Contains(input))
                         .Take(25)
                         .Select(d => new ApplicationCommandOptionChoiceProperties(d, d));

        return new(result);
    }
}
