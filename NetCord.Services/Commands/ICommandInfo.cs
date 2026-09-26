namespace NetCord.Services.Commands;

public interface ICommandInfo<TContext> where TContext : ICommandContext
{
    public IReadOnlyList<string> Aliases { get; }

    public int Priority { get; }

    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }

    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    public ValueTask<CommandExecutionResult> InvokeAsync(ReadOnlyMemory<char> arguments, TContext context, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
}
