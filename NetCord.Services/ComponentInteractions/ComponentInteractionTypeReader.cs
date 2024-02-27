namespace NetCord.Services.ComponentInteractions;

public abstract class ComponentInteractionTypeReader<TContext> : IInteractionTypeReader where TContext : IComponentInteractionContext
{
    public abstract ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
}

internal interface IInteractionTypeReader
{
}
