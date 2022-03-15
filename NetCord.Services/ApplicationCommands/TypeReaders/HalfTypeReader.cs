namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class HalfTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Double;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options) => Task.FromResult((object?)Half.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => 65_504;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => -65_504;
}