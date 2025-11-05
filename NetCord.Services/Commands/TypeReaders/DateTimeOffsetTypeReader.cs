namespace NetCord.Services.Commands.TypeReaders;

public class DateTimeOffsetTypeReader<TContext> : CommandTypeParser<TContext> where TContext : ICommandContext
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(DateTimeOffset.TryParse(input.Span, configuration.CultureInfo, out var result) ? CommandTypeParserResult.Success(result) : CommandTypeParserResult.ParseFail(parameter.Name));
}
