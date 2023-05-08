namespace NetCord.Services.Interactions.TypeReaders;

public class DateTimeTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => Task.FromResult<object?>(DateTime.Parse(input.Span, configuration.CultureInfo));
}
