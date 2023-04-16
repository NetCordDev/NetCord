using System.Text.Json;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.SlashCommands;

public class DDGAutocomplete : IAutocompleteProvider<AutocompleteInteractionContext>
{
    public DDGAutocomplete(HttpClient client)
    {
        _client = client;
    }

    private readonly HttpClient _client;

    public async Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        return JsonDocument.Parse(await (await _client.GetAsync($"https://duckduckgo.com/ac/?q={Uri.EscapeDataString(option.Value!)}")).Content.ReadAsStreamAsync()).RootElement.EnumerateArray().Select(e =>
        {
            var s = e.GetProperty("phrase").GetString()!;
            return new ApplicationCommandOptionChoiceProperties(s, s);
        });
    }
}
