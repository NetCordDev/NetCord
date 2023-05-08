namespace NetCord.Services.Interactions.TypeReaders;

public class DateOnlyTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => Task.FromResult<object?>(DateOnly.Parse(input.Span, provider: configuration.CultureInfo));
}
