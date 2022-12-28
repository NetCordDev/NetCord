using System.Globalization;
using System.Numerics;

namespace NetCord.Services.Interactions.TypeReaders;

public class BigIntegerTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options) => Task.FromResult<object?>(BigInteger.Parse(input.Span, NumberStyles.AllowLeadingSign, options.CultureInfo));
}
