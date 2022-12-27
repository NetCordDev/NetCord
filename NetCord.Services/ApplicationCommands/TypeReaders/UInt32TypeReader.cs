using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class UInt32TypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options) => Task.FromResult((object?)uint.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => uint.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => uint.MinValue;
}
