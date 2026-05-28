using System.Globalization;

namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class IntPtrTypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<ComponentInteractionTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(nint.TryParse(input.Span, NumberStyles.AllowLeadingSign, configuration.CultureInfo, out var result) ? ComponentInteractionTypeReaderResult.Success(result) : ComponentInteractionTypeReaderResult.ParseFail(parameter.Name));
}
