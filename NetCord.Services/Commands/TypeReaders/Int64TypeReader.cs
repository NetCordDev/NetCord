using System.Globalization;

namespace NetCord.Services.Commands.TypeReaders;

public class Int64TypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => Task.FromResult<object?>(long.Parse(input.Span, NumberStyles.AllowLeadingSign, configuration.CultureInfo));
}
