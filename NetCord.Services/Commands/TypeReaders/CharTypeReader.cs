namespace NetCord.Services.Commands.TypeReaders;

public class CharTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options)
    {
        if (input.Length != 1)
            throw new FormatException("Input must be exactly one character long.");

        return Task.FromResult<object?>(input.Span[0]);
    }
}
