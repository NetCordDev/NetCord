namespace NetCord.Services.Interactions.TypeReaders;

public class CharTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (input.Length != 1)
            throw new FormatException("Input must be exactly one character long.");

        return Task.FromResult<object?>(input.Span[0]);
    }
}
