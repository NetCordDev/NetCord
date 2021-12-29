namespace NetCord.Commands;

public interface ITypeReader : ITypeReader<CommandContext>
{
}

public interface ITypeReader<TContext> where TContext : ICommandContext
{
    public Task<object> ReadAsync(string input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options);
}