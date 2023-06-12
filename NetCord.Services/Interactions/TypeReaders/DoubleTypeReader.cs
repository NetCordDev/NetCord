using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class DoubleTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => Task.FromResult<object?>(double.Parse(input.Span, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, configuration.CultureInfo));
}
