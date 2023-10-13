using System.Globalization;

namespace NetCord.Services.Commands.TypeReaders;

public class SByteTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(sbyte.Parse(input.Span, NumberStyles.AllowLeadingSign, configuration.CultureInfo));
}
