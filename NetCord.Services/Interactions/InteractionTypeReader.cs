namespace NetCord.Services.Interactions;

public abstract class InteractionTypeReader<TContext> : IInteractionTypeReader where TContext : InteractionContext
{
    public abstract Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration);
}

internal interface IInteractionTypeReader
{
}
