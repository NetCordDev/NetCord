namespace NetCord.Services.Commands.TypeReaders;

public class DecimalTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options) => Task.FromResult((object?)decimal.Parse(input.Span, provider: options.CultureInfo));
}
