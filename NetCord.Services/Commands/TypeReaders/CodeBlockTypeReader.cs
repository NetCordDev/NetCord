namespace NetCord.Services.Commands.TypeReaders;

public class CodeBlockTypeReader<TContext> : CommandTypeParser<TContext> where TContext : ICommandContext
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(CodeBlock.TryParse(input.Span, out var result) ? CommandTypeParserResult.Success(result) : CommandTypeParserResult.ParseFail(parameter.Name));
}
