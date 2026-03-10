namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class GuidTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<ComponentInteractionTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(Guid.TryParse(input.Span, out var value) ? ComponentInteractionTypeReaderResult.Success(value) : ComponentInteractionTypeReaderResult.ParseFail(parameter.Name));
}
