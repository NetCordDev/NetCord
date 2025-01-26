using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting.AspNetCore;

public class EchoAutocompleteProvider : IAutocompleteProvider<HttpAutocompleteInteractionContext>
{
    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, HttpAutocompleteInteractionContext context)
    {
        return new ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?>(
        [
            new ApplicationCommandOptionChoiceProperties("Hello", "Hello!"),
            new ApplicationCommandOptionChoiceProperties("World", "World!"),
        ]);
    }
}
