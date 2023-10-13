using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class Int32TypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(int.Parse(input.Span, NumberStyles.AllowLeadingSign, configuration.CultureInfo));
}
