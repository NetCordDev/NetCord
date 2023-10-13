using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class ByteTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(byte.Parse(input.Span, NumberStyles.None, configuration.CultureInfo));
}
