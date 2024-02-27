using System.Globalization;

namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class HalfTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(Half.TryParse(input.Span, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, configuration.CultureInfo, out var result) ? TypeReaderResult.Success(result) : TypeReaderResult.ParseFail(parameter.Name));
}
