namespace NetCord.Services.Interactions.TypeReaders;

public class CharTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (input.Length != 1)
            throw new FormatException("Input must be exactly one character long.");

        return new(input.Span[0]);
    }
}
