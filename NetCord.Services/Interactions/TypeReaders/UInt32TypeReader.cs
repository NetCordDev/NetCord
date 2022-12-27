using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class UInt32TypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options) => Task.FromResult((object?)uint.Parse(input.Span, NumberStyles.None, options.CultureInfo));
}
