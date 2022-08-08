namespace NetCord.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public abstract class PreconditionAttribute<TContext> : Attribute, IPreconditionAttribute where TContext : IContext
{
    public abstract ValueTask EnsureCanExecuteAsync(TContext context);
}

internal interface IPreconditionAttribute
{
}