namespace NetCord.Services.Interactions.TypeReaders;

public class TimeOnlyTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration) => Task.FromResult<object?>(TimeOnly.Parse(input.Span, configuration.CultureInfo));
}
