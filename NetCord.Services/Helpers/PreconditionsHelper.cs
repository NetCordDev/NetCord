using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class PreconditionsHelper
{
    public static IReadOnlyList<PreconditionAttribute<TContext>> GetPreconditions<TContext>(MemberInfo mainMember, Attribute[] mainMemberAttributes, params MemberInfo[] members)
    {
        List<PreconditionAttribute<TContext>> preconditions = [];

        foreach (var member in members)
        {
            foreach (var attribute in member.GetCustomAttributes())
            {
                if (!CheckPrecondition<TContext>(attribute, member, out var precondition))
                    continue;

                preconditions.Add(precondition);
            }
        }

        foreach (var attribute in mainMemberAttributes)
        {
            if (!CheckPrecondition<TContext>(attribute, mainMember, out var precondition))
                continue;

            preconditions.Add(precondition);
        }

        return preconditions;
    }

    public static IReadOnlyList<ParameterPreconditionAttribute<TContext>> GetParameterPreconditions<TContext>(Attribute[] attributes, MethodInfo method)
    {
        List<ParameterPreconditionAttribute<TContext>> preconditions = [];

        foreach (var attribute in attributes)
        {
            if (!CheckParameterPrecondition<TContext>(attribute, method, out var precondition))
                continue;

            preconditions.Add(precondition);
        }

        return preconditions;
    }

    private static bool CheckPrecondition<TContext>(Attribute attribute, MemberInfo member, [MaybeNullWhen(false)] out PreconditionAttribute<TContext> precondition)
    {
        if (attribute is not IPreconditionAttribute iPreconditionAttribute)
        {
            precondition = null;
            return false;
        }

        if (iPreconditionAttribute is not PreconditionAttribute<TContext> preconditionAttribute)
            throw new InvalidDefinitionException($"The '{nameof(TContext)}' used for '{iPreconditionAttribute.GetType()}' does not match the expected context '{typeof(TContext)}'", member);

        precondition = preconditionAttribute;
        return true;
    }

    private static bool CheckParameterPrecondition<TContext>(Attribute attribute, MemberInfo member, [MaybeNullWhen(false)] out ParameterPreconditionAttribute<TContext> precondition)
    {
        if (attribute is not IParameterPreconditionAttribute iPreconditionAttribute)
        {
            precondition = null;
            return false;
        }

        if (iPreconditionAttribute is not ParameterPreconditionAttribute<TContext> preconditionAttribute)
            throw new InvalidDefinitionException($"The '{nameof(TContext)}' used for '{iPreconditionAttribute.GetType()}' does not match the expected context '{typeof(TContext)}'", member);

        precondition = preconditionAttribute;
        return true;
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
