using System.Globalization;

namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class SByteTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override ValueTask<TypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(sbyte.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result) ? TypeReaderResult.Success(result) : TypeReaderResult.ParseFail(parameter.Name));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => sbyte.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => sbyte.MinValue;
}
