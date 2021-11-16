using System.Reflection;

namespace NetCord.Interactions;

internal class InteractionInfo
{
    public Type DeclaringType { get; }
    public Func<object, Task> InvokeAsync { get; }

    public InteractionInfo(MethodInfo methodInfo)
    {
        if (methodInfo.ReturnType != typeof(Task))
            throw new InvalidInteractionDefinitionException($"Interactions must return {typeof(Task).FullName} | {methodInfo.DeclaringType.FullName}.{methodInfo.Name}");

        InvokeAsync = (obj) =>
        {
            try
            {
                return (Task)methodInfo.Invoke(obj, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        };
        DeclaringType = methodInfo.DeclaringType;
    }
}