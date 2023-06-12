using System.Reflection;

namespace NetCord.Services;

internal static class PreconditionAttributeHelper
{
    public static IReadOnlyList<PreconditionAttribute<TContext>> GetPreconditionAttributes<TContext>(Type declaringType, MethodInfo method)
    {
        List<PreconditionAttribute<TContext>> preconditions = new();
        foreach (var a in declaringType.GetCustomAttributes())
            AddPreconditionIfValid(a);
        foreach (var a2 in method.GetCustomAttributes())
            AddPreconditionIfValid(a2);

        return preconditions;

        void AddPreconditionIfValid(Attribute attribute)
        {
            if (attribute is IPreconditionAttribute iPreconditionAttribute)
            {
                if (iPreconditionAttribute is PreconditionAttribute<TContext> preconditionAttribute)
                    preconditions.Add(preconditionAttribute);
                else
                    throw new InvalidDefinitionException($"'{iPreconditionAttribute.GetType()}' has invalid '{nameof(TContext)}'.", method);
            }
        }
    }
}
