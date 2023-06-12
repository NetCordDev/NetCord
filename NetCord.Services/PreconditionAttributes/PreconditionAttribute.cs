namespace NetCord.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public abstract class PreconditionAttribute<TContext> : Attribute, IPreconditionAttribute
{
    public abstract ValueTask EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider);
}

internal interface IPreconditionAttribute
{
}
