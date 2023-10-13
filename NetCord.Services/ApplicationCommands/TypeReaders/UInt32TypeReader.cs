using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class UInt32TypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override ValueTask<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(uint.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => uint.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => uint.MinValue;
}
