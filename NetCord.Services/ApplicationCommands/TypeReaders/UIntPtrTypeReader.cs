using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class UIntPtrTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options) => Task.FromResult((object?)nuint.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture));

#pragma warning disable IDE0004
    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => Math.Min((ulong)nuint.MaxValue, Discord.ApplicationCommandOptionMaxValue);
#pragma warning restore IDE0004

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => nuint.MinValue;
}
