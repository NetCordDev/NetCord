namespace NetCord.Services.Commands.TypeReaders;

public class UriTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options) => Task.FromResult<object?>(new Uri(input.ToString(), UriKind.Absolute));
}
