using System.Globalization;

namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class HalfTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<ComponentInteractionTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(Half.TryParse(input.Span, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, configuration.CultureInfo, out var result) ? ComponentInteractionTypeReaderResult.Success(result) : ComponentInteractionTypeReaderResult.ParseFail(parameter.Name));
}
