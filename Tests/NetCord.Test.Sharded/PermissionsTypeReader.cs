using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Sharded;

internal class PermissionsTypeReader : SlashCommandTypeReader<SlashCommandContext>
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override ValueTask<SlashCommandTypeReaderResult> ReadAsync(string value, SlashCommandContext context, SlashCommandParameter<SlashCommandContext> parameter, ApplicationCommandServiceConfiguration<SlashCommandContext> configuration, IServiceProvider? serviceProvider)
    {
        return ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var result)
            ? new(SlashCommandTypeReaderResult.Success((Permissions)result))
            : new(SlashCommandTypeReaderResult.ParseFail(parameter.Name));
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public override Type? AutocompleteProviderType => typeof(PermissionsAutocompleteProvider);

    public override double? GetMinValue(SlashCommandParameter<SlashCommandContext> parameter, ApplicationCommandServiceConfiguration<SlashCommandContext> configuration) => 0;
}

internal class PermissionsAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
{
    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        return new(Enum.GetNames<Permissions>().Where(p => p.Contains(option.Value!)).Select(p => new ApplicationCommandOptionChoiceProperties(p, (double)Enum.Parse<Permissions>(p))).Take(25));
    }
}
