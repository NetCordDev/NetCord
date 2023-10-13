using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class UIntPtrTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(nuint.Parse(input.Span, NumberStyles.None, configuration.CultureInfo));
}
