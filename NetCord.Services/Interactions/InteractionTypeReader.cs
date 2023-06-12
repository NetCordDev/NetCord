namespace NetCord.Services.Interactions;

public abstract class InteractionTypeReader<TContext> : IInteractionTypeReader where TContext : IInteractionContext
{
    public abstract Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
}

internal interface IInteractionTypeReader
{
}
