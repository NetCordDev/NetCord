using System.Globalization;

namespace NetCord.Services.Commands.TypeReaders;

public class SingleTypeReader<TContext> : CommandTypeParser<TContext> where TContext : ICommandContext
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(float.TryParse(input.Span, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, configuration.CultureInfo, out var result) ? CommandTypeParserResult.Success(result) : CommandTypeParserResult.ParseFail(parameter.Name));
}
