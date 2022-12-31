using System.Globalization;

namespace NetCord.Services.Commands.TypeReaders;

public class Int32TypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration) => Task.FromResult<object?>(int.Parse(input.Span, NumberStyles.AllowLeadingSign, configuration.CultureInfo));
}
