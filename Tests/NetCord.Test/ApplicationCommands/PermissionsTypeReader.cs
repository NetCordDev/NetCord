using System.Globalization;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.SlashCommands;

internal class PermissionsTypeReader : SlashCommandTypeReader<SlashCommandContext>
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, SlashCommandContext context, SlashCommandParameter<SlashCommandContext> parameter, ApplicationCommandServiceConfiguration<SlashCommandContext> configuration, IServiceProvider? serviceProvider)
    {
        return Task.FromResult<object?>((Permissions)ulong.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture));
    }

    public override Type? AutocompleteProviderType => typeof(PermissionsAutocompleteProvider);

    public override double? GetMinValue(SlashCommandParameter<SlashCommandContext> parameter, ApplicationCommandServiceConfiguration<SlashCommandContext> configuration) => 0;
}

internal class PermissionsAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
{
    public Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        return Task.FromResult<IEnumerable<ApplicationCommandOptionChoiceProperties>?>(Enum.GetNames<Permissions>().Where(p => p.Contains(option.Value!)).Select(p => new ApplicationCommandOptionChoiceProperties(p, (double)Enum.Parse<Permissions>(p))).Take(25));
    }
}
