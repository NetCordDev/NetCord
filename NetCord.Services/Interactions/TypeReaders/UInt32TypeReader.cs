using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class UInt32TypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => Task.FromResult<object?>(uint.Parse(input.Span, NumberStyles.None, configuration.CultureInfo));
}
