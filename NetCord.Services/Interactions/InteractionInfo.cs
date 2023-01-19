using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetCord.Services.Interactions;

public class InteractionInfo<TContext> where TContext : InteractionContext
{
    public Type DeclaringType { get; }
    public bool Static { get; }
    public IReadOnlyList<InteractionParameter<TContext>> Parameters { get; }
    public Func<object?[], Task> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    internal InteractionInfo(MethodInfo method, InteractionServiceConfiguration<TContext> configuration)
    {
        if (method.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Interactions must return '{typeof(Task).FullName}'.", method);

        DeclaringType = method.DeclaringType!;

        var parameters = method.GetParameters();
        var parametersLength = parameters.Length;

        Type[] types;
        int start;
        if (method.IsStatic)
        {
            Static = true;
            types = new Type[parametersLength + 1];
            start = 0;
        }
        else
        {
            types = new Type[parametersLength + 2];
            types[0] = DeclaringType;
            start = 1;
        }
        types[^1] = typeof(Task);

        var p = new InteractionParameter<TContext>[parametersLength];
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            p[i] = new(parameter, method, configuration);
            types[start++] = parameter.ParameterType;
        }
        Parameters = p;

        var invoke = method.CreateDelegate(Expression.GetDelegateType(types)).DynamicInvoke;
        InvokeAsync = Unsafe.As<Func<object?[], Task>>((object?[] p) =>
        {
            try
            {
                return invoke(p);
            }
            catch (Exception ex)
            {
                throw ex.InnerException!;
            }
        });

        Preconditions = PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(DeclaringType, method);
    }

    internal async Task EnsureCanExecuteAsync(TContext context)
    {
        foreach (var preconditionAttribute in Preconditions)
            await preconditionAttribute.EnsureCanExecuteAsync(context).ConfigureAwait(false);
    }
}
