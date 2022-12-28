using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class UInt16TypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options) => Task.FromResult<object?>(ushort.Parse(input.Span, NumberStyles.None, options.CultureInfo));
}
