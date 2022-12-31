using System.Globalization;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.SlashCommands;

internal class PermissionsTypeReader : SlashCommandTypeReader<SlashCommandContext>
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, SlashCommandContext context, SlashCommandParameter<SlashCommandContext> parameter, ApplicationCommandServiceConfiguration<SlashCommandContext> configuration)
    {
        return Task.FromResult((object?)(Permissions)ulong.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture));
    }

    public override IAutocompleteProvider AutocompleteProvider => new PermissionsAutocompleteProvider();

    public override double? GetMinValue(SlashCommandParameter<SlashCommandContext> parameter) => 0;
}

internal class PermissionsAutocompleteProvider : IAutocompleteProvider
{
    public Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, ApplicationCommandAutocompleteInteraction interaction)
    {
        return Task.FromResult((IEnumerable<ApplicationCommandOptionChoiceProperties>?)Enum.GetNames<Permissions>().Where(p => p.Contains(option.Value!)).Select(p => new ApplicationCommandOptionChoiceProperties(p, (double)Enum.Parse<Permissions>(p))).Take(25));
    }
}
