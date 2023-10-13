using System.Globalization;

namespace NetCord.Services.Commands.TypeReaders;

public class IntPtrTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(nint.Parse(input.Span, NumberStyles.AllowLeadingSign, configuration.CultureInfo));
}
