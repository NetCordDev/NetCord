namespace NetCord.Services.Interactions.TypeReaders;

public class UriTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options) => Task.FromResult((object?)new Uri(input.ToString(), UriKind.Absolute));
}
