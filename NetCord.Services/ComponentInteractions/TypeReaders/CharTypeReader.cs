namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class CharTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (input.Length != 1)
            return new(TypeReaderResult.Fail("Input must be exactly one character long."));

        return new(TypeReaderResult.Success(input.Span[0]));
    }
}
