using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class IntPtrTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration) => Task.FromResult<object?>(nint.Parse(input.Span, NumberStyles.AllowLeadingSign, configuration.CultureInfo));
}
