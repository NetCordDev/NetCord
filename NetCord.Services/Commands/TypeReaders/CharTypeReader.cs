namespace NetCord.Services.Commands.TypeReaders;

public class CharTypeReader<TContext> : CommandTypeParser<TContext> where TContext : ICommandContext
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (input.Length != 1)
            return new(CommandTypeParserResult.Fail("Input must be exactly one character long."));

        return new(CommandTypeParserResult.Success(input.Span[0]));
    }
}
