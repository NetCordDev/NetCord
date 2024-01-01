using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class PreconditionsHelper
{
    public static IReadOnlyList<PreconditionAttribute<TContext>> GetPreconditions<TContext>(params MemberInfo[] members)
    {
        List<PreconditionAttribute<TContext>> preconditions = [];

        foreach (var member in members)
        {
            foreach (var attribute in member.GetCustomAttributes())
            {
                if (attribute is not IPreconditionAttribute iPreconditionAttribute)
                    continue;

                if (iPreconditionAttribute is not PreconditionAttribute<TContext> preconditionAttribute)
                    throw new InvalidDefinitionException($"'{iPreconditionAttribute.GetType()}' has invalid '{nameof(TContext)}'.", member);

                preconditions.Add(preconditionAttribute);
            }
        }

        return preconditions;
    }

    public static IReadOnlyList<ParameterPreconditionAttribute<TContext>> GetParameterPreconditions<TContext>(IEnumerable<Attribute> attributes, MethodInfo method)
    {
        List<ParameterPreconditionAttribute<TContext>> preconditions = [];

        foreach (var attribute in attributes)
        {
            if (attribute is not IParameterPreconditionAttribute iPreconditionAttribute)
                continue;

            if (iPreconditionAttribute is not ParameterPreconditionAttribute<TContext> preconditionAttribute)
                throw new InvalidDefinitionException($"'{iPreconditionAttribute.GetType()}' has invalid '{nameof(TContext)}'.", method);

            preconditions.Add(preconditionAttribute);
        }

        return preconditions;
    }

    public static async ValueTask<PreconditionResult> EnsureCanExecuteAsync<TContext>(IReadOnlyList<PreconditionAttribute<TContext>> preconditions, TContext context, IServiceProvider? serviceProvider)
    {
        int count = preconditions.Count;
        for (int i = 0; i < count; i++)
        {
            var precondition = preconditions[i];

            PreconditionResult result;
            try
            {
                result = await precondition.EnsureCanExecuteAsync(context, serviceProvider).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new PreconditionExceptionResult(ex);
            }

            if (result is IFailResult)
                return result;
        }

        return PreconditionResult.Success;
    }

    public static async ValueTask<PreconditionResult> EnsureCanExecuteAsync<TContext>(IReadOnlyList<ParameterPreconditionAttribute<TContext>> preconditions, object? value, TContext context, IServiceProvider? serviceProvider)
    {
        int count = preconditions.Count;
        for (int i = 0; i < count; i++)
        {
            var precondition = preconditions[i];

            PreconditionResult result;
            try
            {
                result = await precondition.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new PreconditionExceptionResult(ex);
            }

            if (result is IFailResult)
                return result;
        }

        return PreconditionResult.Success;
    }
}
