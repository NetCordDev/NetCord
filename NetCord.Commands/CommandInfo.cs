using System.Reflection;

namespace NetCord.Commands;

public record CommandInfo<TContext> where TContext : ICommandContext
{
    public Type DeclaringType { get; }
    public CommandParameter<TContext>[] CommandParameters { get; }
    public int Priority { get; }
    public Context RequiredContext { get; }
    public Func<object, object[], Task> InvokeAsync { get; }

    public CommandInfo(MethodInfo methodInfo, CommandAttribute attribute, Dictionary<Type, Func<string, TContext, CommandServiceOptions<TContext>, Task<object>>> typeReaders)
    {
        if (methodInfo.ReturnType != typeof(Task))
            throw new InvalidCommandDefinitionException($"Commands must return {typeof(Task).FullName} | {methodInfo.DeclaringType.FullName}.{methodInfo.Name}");

        Priority = attribute.Priority;
        RequiredContext = attribute.RequiredContext;
        DeclaringType = methodInfo.DeclaringType;

        var parameters = methodInfo.GetParameters();
        int parametersLength = parameters.Length;
        CommandParameters = new CommandParameter<TContext>[parametersLength];
        bool hasDefaultValue = false;
        for (int i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidCommandDefinitionException($"Optional parameters must appear after all required parameters | {methodInfo.DeclaringType.FullName}.{methodInfo.Name}");
            CommandParameters[i] = new(parameter, typeReaders);
        }

        InvokeAsync = (obj, parameters) =>
        {
            try
            {
                return (Task)methodInfo.Invoke(obj, parameters);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        };
    }
}