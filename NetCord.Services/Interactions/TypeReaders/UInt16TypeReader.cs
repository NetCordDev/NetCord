namespace NetCord.Services.Interactions.TypeReaders;

public class UInt16TypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(string input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options) => Task.FromResult((object?)ushort.Parse(input, options.CultureInfo));
}