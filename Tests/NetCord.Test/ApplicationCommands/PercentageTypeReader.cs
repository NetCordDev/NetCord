using System.Globalization;

using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

internal class PercentageTypeReader : SlashCommandTypeReader<SlashCommandContext>
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override ValueTask<SlashCommandTypeReaderResult> ReadAsync(string value, SlashCommandContext context, SlashCommandParameter<SlashCommandContext> parameter, ApplicationCommandServiceConfiguration<SlashCommandContext> configuration, IServiceProvider? serviceProvider)
        => new(byte.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var result) ? SlashCommandTypeReaderResult.Success(result) : SlashCommandTypeReaderResult.ParseFail(parameter.Name));

    public override double? GetMaxValue(SlashCommandParameter<SlashCommandContext> parameter, ApplicationCommandServiceConfiguration<SlashCommandContext> configuration) => 100;

    public override double? GetMinValue(SlashCommandParameter<SlashCommandContext> parameter, ApplicationCommandServiceConfiguration<SlashCommandContext> configuration) => 0;
}
