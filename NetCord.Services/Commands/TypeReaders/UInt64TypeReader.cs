using System.Globalization;

namespace NetCord.Services.Commands.TypeReaders;

public class UInt64TypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options) => Task.FromResult<object?>(ulong.Parse(input.Span, NumberStyles.None, options.CultureInfo));
}
