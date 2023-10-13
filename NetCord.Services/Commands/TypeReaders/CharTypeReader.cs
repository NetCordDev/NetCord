namespace NetCord.Services.Commands.TypeReaders;

public class CharTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (input.Length != 1)
            throw new FormatException("Input must be exactly one character long.");

        return new(input.Span[0]);
    }
}
