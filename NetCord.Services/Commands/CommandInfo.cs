using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetCord.Services.Commands;

public class CommandInfo<TContext> where TContext : ICommandContext
{
    public Type DeclaringType { get; }
    public bool Static { get; }
    public IReadOnlyList<CommandParameter<TContext>> Parameters { get; }
    public int Priority { get; }
    public Func<object?[], Task> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    internal CommandInfo(MethodInfo method, CommandAttribute attribute, CommandServiceConfiguration<TContext> configuration)
    {
        if (method.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Commands must return '{typeof(Task).FullName}'.", method);

        Priority = attribute.Priority;
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

        var p = new CommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];

            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException($"Optional parameters must appear after all required parameters.", method);

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
        var count = Preconditions.Count;
        for (var i = 0; i < count; i++)
        {
            PreconditionAttribute<TContext>? preconditionAttribute = Preconditions[i];
            await preconditionAttribute.EnsureCanExecuteAsync(context).ConfigureAwait(false);
        }
    }
}
