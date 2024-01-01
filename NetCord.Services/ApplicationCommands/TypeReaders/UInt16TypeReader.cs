using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class UInt16TypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override ValueTask<TypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(ushort.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var result) ? TypeReaderResult.Success(result) : TypeReaderResult.ParseFail(parameter.Name));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => ushort.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => ushort.MinValue;
}
