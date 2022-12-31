using System.Globalization;

namespace NetCord.Services.Commands.TypeReaders;

public class ByteTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration) => Task.FromResult<object?>(byte.Parse(input.Span, NumberStyles.None, configuration.CultureInfo));
}
