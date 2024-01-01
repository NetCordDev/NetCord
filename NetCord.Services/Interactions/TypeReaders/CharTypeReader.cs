namespace NetCord.Services.Interactions.TypeReaders;

public class CharTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (input.Length != 1)
            return new(TypeReaderResult.Fail("Input must be exactly one character long."));

        return new(TypeReaderResult.Success(input.Span[0]));
    }
}
