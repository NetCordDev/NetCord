namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class DateOnlyTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<ComponentInteractionTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(DateOnly.TryParse(input.Span, provider: configuration.CultureInfo, out var result) ? ComponentInteractionTypeReaderResult.Success(result) : ComponentInteractionTypeReaderResult.ParseFail(parameter.Name));
}
