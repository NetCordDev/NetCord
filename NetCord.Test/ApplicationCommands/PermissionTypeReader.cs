using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.SlashCommands;

internal class PermissionTypeReader : SlashCommandTypeReader<SlashCommandContext>
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, SlashCommandContext context, SlashCommandParameter<SlashCommandContext> parameter, ApplicationCommandServiceOptions<SlashCommandContext> options)
    {
        return Task.FromResult((object?)(Permission)ulong.Parse(value));
    }

    public override IAutocompleteProvider GetAutocompleteProvider(SlashCommandParameter<SlashCommandContext> parameter)
    {
        return new PermissionAutocompleteProvider();
    }

    public override double? GetMinValue(SlashCommandParameter<SlashCommandContext> parameter) => 0;
}

internal class PermissionAutocompleteProvider : IAutocompleteProvider
{
    public Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, ApplicationCommandAutocompleteInteraction interaction)
    {
        return Task.FromResult((IEnumerable<ApplicationCommandOptionChoiceProperties>?)Enum.GetNames<Permission>().Where(p => p.Contains(option.Value!)).Select(p => new ApplicationCommandOptionChoiceProperties(p, (double)Enum.Parse<Permission>(p))).Take(25));
    }
}