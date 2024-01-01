namespace NetCord.Services.Commands.TypeReaders;

public class CharTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (input.Length != 1)
            return new(TypeReaderResult.Fail("Input must be exactly one character long."));

        return new(TypeReaderResult.Success(input.Span[0]));
    }
}
