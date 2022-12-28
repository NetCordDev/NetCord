using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class SByteTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options) => Task.FromResult<object?>(sbyte.Parse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => sbyte.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => sbyte.MinValue;
}
