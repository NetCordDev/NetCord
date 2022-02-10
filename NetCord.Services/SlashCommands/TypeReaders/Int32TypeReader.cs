namespace NetCord.Services.SlashCommands.TypeReaders;

public class Int32TypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : ISlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options) => Task.FromResult((object)int.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => int.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => int.MinValue;
}