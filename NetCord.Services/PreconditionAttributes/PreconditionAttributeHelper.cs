using System.Reflection;

namespace NetCord.Services;

internal static class PreconditionAttributeHelper
{
    public static IList<PreconditionAttribute<TContext>> GetPreconditionAttributes<TContext>(MethodInfo methodInfo, Type declaringType) where TContext : IContext
    {
        List<PreconditionAttribute<TContext>> preconditions = new();
        foreach (var a in methodInfo.GetCustomAttributes())
            AddPreconditionIfValid(a);
        foreach (var a2 in declaringType.GetCustomAttributes())
            AddPreconditionIfValid(a2);

        return preconditions;

        void AddPreconditionIfValid(Attribute attribute)
        {
            if (attribute is IPreconditionAttribute iPreconditionAttribute)
            {
                if (iPreconditionAttribute is PreconditionAttribute<TContext> preconditionAttribute)
                    preconditions.Add(preconditionAttribute);
                else
                    throw new InvalidDefinitionException($"'{iPreconditionAttribute.GetType().FullName}' has invalid '{nameof(TContext)}'.", methodInfo);
            }
        }
    }
}