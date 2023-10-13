using System.Globalization;
using System.Numerics;

namespace NetCord.Services.Interactions.TypeReaders;

public class BigIntegerTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(BigInteger.Parse(input.Span, NumberStyles.AllowLeadingSign, configuration.CultureInfo));
}
