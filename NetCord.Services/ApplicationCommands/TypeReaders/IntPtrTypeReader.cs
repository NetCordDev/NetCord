using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class IntPtrTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => Task.FromResult<object?>(nint.Parse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => Math.Min(nint.MaxValue, Discord.ApplicationCommandOptionMaxValue);

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => Math.Max(nint.MinValue, Discord.ApplicationCommandOptionMinValue);
}
