using System.Reflection;

namespace NetCord.Services;

internal static class ParameterPreconditionAttributeHelper
{
    public static IReadOnlyList<ParameterPreconditionAttribute<TContext>> GetPreconditionAttributes<TContext>(IEnumerable<Attribute> attributes, MethodInfo method) where TContext : IContext
    {
        List<ParameterPreconditionAttribute<TContext>> preconditions = new();
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
}
