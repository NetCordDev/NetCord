namespace NetCord.Services.ApplicationCommands;

public interface IAutocompleteProvider
{
    public Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, ApplicationCommandAutocompleteInteraction interaction);
}