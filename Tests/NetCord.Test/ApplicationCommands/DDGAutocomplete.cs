using System.Text.Json;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

public class DDGAutocomplete(HttpClient client) : IAutocompleteProvider<AutocompleteInteractionContext>
{
    public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        return JsonDocument.Parse(await (await client.GetAsync($"https://duckduckgo.com/ac/?q={Uri.EscapeDataString(option.Value!)}")).Content.ReadAsStreamAsync()).RootElement.EnumerateArray().Select(e =>
        {
            var s = e.GetProperty("phrase").GetString()!;
            return new ApplicationCommandOptionChoiceProperties(s, s);
        });
    }
}
