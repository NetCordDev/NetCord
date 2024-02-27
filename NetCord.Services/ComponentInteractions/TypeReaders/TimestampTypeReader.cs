namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class TimestampTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(Timestamp.TryParse(input.Span, out var result) ? TypeReaderResult.Success(result) : TypeReaderResult.ParseFail(parameter.Name));
}
