using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class UInt16TypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(ushort.Parse(input.Span, NumberStyles.None, configuration.CultureInfo));
}
