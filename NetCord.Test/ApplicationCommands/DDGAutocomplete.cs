using System.Text.Json;

using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.SlashCommands;

public class DDGAutocomplete : IAutocompleteProvider
{
    private readonly HttpClient _client = new();

    public async Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, ApplicationCommandAutocompleteInteraction interaction)
    {
        return JsonDocument.Parse(await (await _client.GetAsync($"https://duckduckgo.com/ac/?q={Uri.EscapeDataString(option.Value!)}")).Content.ReadAsStreamAsync()).RootElement.EnumerateArray().Select(e =>
        {
            string s = e.GetProperty("phrase").GetString()!;
            return new ApplicationCommandOptionChoiceProperties(s, s);
        });
    }
}