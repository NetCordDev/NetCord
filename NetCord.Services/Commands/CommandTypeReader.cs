namespace NetCord.Services.Commands;

public abstract class CommandTypeReader<TContext> : ICommandTypeReader where TContext : ICommandContext
{
    public abstract Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
}

internal interface ICommandTypeReader
{
}
