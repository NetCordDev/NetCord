namespace NetCord.Services.Interactions.TypeReaders;

public class DateOnlyTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(DateOnly.Parse(input.Span, provider: configuration.CultureInfo));
}
