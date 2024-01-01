namespace NetCord.Services;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public abstract class ParameterPreconditionAttribute<TContext> : Attribute, IParameterPreconditionAttribute
{
    public abstract ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider);
}

internal interface IParameterPreconditionAttribute
{
}
