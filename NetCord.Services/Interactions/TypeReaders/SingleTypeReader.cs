using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class SingleTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration) => Task.FromResult<object?>(float.Parse(input.Span, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, configuration.CultureInfo));
}
