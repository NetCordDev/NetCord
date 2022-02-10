namespace NetCord.Services.SlashCommands.TypeReaders;

public class SByteTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : ISlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options) => Task.FromResult((object)sbyte.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => sbyte.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => sbyte.MinValue;
}