using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class PreconditionsHelper
{
    public static IReadOnlyList<PreconditionAttribute<TContext>> GetPreconditions<TContext>(params MemberInfo[] members)
    {
        List<PreconditionAttribute<TContext>> preconditions = [];

        foreach (MemberInfo member in members)
            foreach (var attribute in member.GetCustomAttributes())
            {
                if (attribute is not IPreconditionAttribute iPreconditionAttribute)
                    continue;

                if (iPreconditionAttribute is not PreconditionAttribute<TContext> preconditionAttribute)
                    throw new InvalidDefinitionException($"'{iPreconditionAttribute.GetType()}' has invalid '{nameof(TContext)}'.", member);

                preconditions.Add(preconditionAttribute);
            }

        return preconditions;
    }

    public static IReadOnlyList<ParameterPreconditionAttribute<TContext>> GetParameterPreconditions<TContext>(IEnumerable<Attribute> attributes, MethodInfo method)
    {
        List<ParameterPreconditionAttribute<TContext>> preconditions = [];
        foreach (var a in attributes)
            AddPreconditionIfValid(a);

        return preconditions;

        void AddPreconditionIfValid(Attribute attribute)
        {
            if (attribute is IParameterPreconditionAttribute iPreconditionAttribute)
            {
                if (iPreconditionAttribute is ParameterPreconditionAttribute<TContext> preconditionAttribute)
                    preconditions.Add(preconditionAttribute);
                else
                    throw new InvalidDefinitionException($"'{iPreconditionAttribute.GetType()}' has invalid '{nameof(TContext)}'.", method);
            }
        }
    }

    public static async ValueTask EnsureCanExecuteAsync<TContext>(IReadOnlyList<PreconditionAttribute<TContext>> preconditions, TContext context, IServiceProvider? serviceProvider)
    {
        int count = preconditions.Count;
        for (int i = 0; i < count; i++)
        {
            var precondition = preconditions[i];
            await precondition.EnsureCanExecuteAsync(context, serviceProvider).ConfigureAwait(false);
        }
    }

    public static async ValueTask EnsureCanExecuteAsync<TContext>(IReadOnlyList<ParameterPreconditionAttribute<TContext>> preconditions, object? value, TContext context, IServiceProvider? serviceProvider)
    {
        int count = preconditions.Count;
        for (int i = 0; i < count; i++)
        {
            var precondition = preconditions[i];
            await precondition.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
        }
    }
}
