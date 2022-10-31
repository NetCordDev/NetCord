namespace NetCord.Services;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public abstract class ParameterPreconditionAttribute<TContext> : Attribute, IParameterPreconditionAttribute where TContext : IContext
{
    public abstract ValueTask EnsureCanExecuteAsync(object? value, TContext context);
}

internal interface IParameterPreconditionAttribute
{
}
