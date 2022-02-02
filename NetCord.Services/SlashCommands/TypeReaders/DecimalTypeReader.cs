namespace NetCord.Services.SlashCommands.TypeReaders;

public class DecimalTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : BaseSlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Double;

    public override Task<object> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options) => Task.FromResult((object)decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => (double)decimal.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => (double)decimal.MinValue;
}