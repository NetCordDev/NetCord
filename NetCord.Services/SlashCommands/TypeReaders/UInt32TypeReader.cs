namespace NetCord.Services.SlashCommands.TypeReaders;

public class UInt32TypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : ISlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options) => Task.FromResult((object)uint.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => uint.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => uint.MinValue;
}