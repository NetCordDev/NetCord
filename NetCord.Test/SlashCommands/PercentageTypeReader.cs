using NetCord.Services.SlashCommands;

namespace NetCord.Test.SlashCommands;

internal class PercentageTypeReader : SlashCommandTypeReader<SlashCommandContext>
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, SlashCommandContext context, SlashCommandParameter<SlashCommandContext> parameter, SlashCommandServiceOptions<SlashCommandContext> options)
        => Task.FromResult((object?)byte.Parse(value));

    public override double? GetMaxValue(SlashCommandParameter<SlashCommandContext> parameter) => 100;

    public override double? GetMinValue(SlashCommandParameter<SlashCommandContext> parameter) => 0;
}