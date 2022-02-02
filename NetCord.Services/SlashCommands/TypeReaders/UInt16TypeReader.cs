namespace NetCord.Services.SlashCommands.TypeReaders;

public class UInt16TypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : BaseSlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options) => Task.FromResult((object)ushort.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => ushort.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => ushort.MinValue;
}