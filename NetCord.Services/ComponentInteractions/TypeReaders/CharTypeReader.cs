namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class CharTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<ComponentInteractionTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (input.Length != 1)
            return new(ComponentInteractionTypeReaderResult.Fail("Input must be exactly one character long."));

        return new(ComponentInteractionTypeReaderResult.Success(input.Span[0]));
    }
}
