using System.Globalization;

namespace NetCord.Services.Commands.TypeReaders;

public class UInt16TypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration) => Task.FromResult<object?>(ushort.Parse(input.Span, NumberStyles.None, configuration.CultureInfo));
}
