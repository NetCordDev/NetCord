using System.Globalization;

namespace NetCord.Services.ComponentInteractions.TypeReaders;

public class UInt128TypeReader<TContext> : ComponentInteractionTypeReader<TContext> where TContext : IComponentInteractionContext
{
    public override ValueTask<ComponentInteractionTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionParameter<TContext> parameter, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(UInt128.TryParse(input.Span, NumberStyles.None, configuration.CultureInfo, out var result) ? ComponentInteractionTypeReaderResult.Success(result) : ComponentInteractionTypeReaderResult.ParseFail(parameter.Name));
}
