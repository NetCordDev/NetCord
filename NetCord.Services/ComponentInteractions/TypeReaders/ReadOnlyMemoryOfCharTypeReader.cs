namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class ReadOnlyMemoryOfCharTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<ComponentInteractionTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(ComponentInteractionTypeReaderResult.Success(input));
}
