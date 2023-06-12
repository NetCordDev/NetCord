namespace NetCord.Services.Interactions.TypeReaders;

public class DateTimeOffsetTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => Task.FromResult<object?>(DateTimeOffset.Parse(input.Span, configuration.CultureInfo));
}
