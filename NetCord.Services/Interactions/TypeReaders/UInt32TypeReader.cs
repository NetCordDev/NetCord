namespace NetCord.Services.Interactions.TypeReaders;

public class UInt32TypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object> ReadAsync(string input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options) => Task.FromResult((object)uint.Parse(input, options.CultureInfo));
}