namespace NetCord.Services.SlashCommands.TypeReaders;

public class IntPtrTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : ISlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options) => Task.FromResult((object?)nint.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => Math.Min(nint.MaxValue, Discord.ApplicationCommandOptionMaxValue);

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => Math.Max(nint.MinValue, Discord.ApplicationCommandOptionMinValue);
}