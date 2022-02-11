namespace NetCord.Services.Interactions.TypeReaders;

public class ByteTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(string input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options) => Task.FromResult((object?)byte.Parse(input, options.CultureInfo));
}