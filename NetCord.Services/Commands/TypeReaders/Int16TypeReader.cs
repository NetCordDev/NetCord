namespace NetCord.Services.Commands.TypeReaders;

public class Int16TypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(string input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options) => Task.FromResult((object?)short.Parse(input, options.CultureInfo));
}