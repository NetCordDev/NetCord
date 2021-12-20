using System.Reflection;

namespace NetCord.Commands;

public record CommandInfo<TContext> where TContext : ICommandContext
{
    public Type DeclaringType { get; }
    public CommandParameter<TContext>[] CommandParameters { get; }
    public int Priority { get; }
    public Context RequiredContext { get; }
    public Permission RequiredBotPermissions { get; }
    public Permission RequiredBotChannelPermissions { get; }
    public Permission RequiredUserPermissions { get; }
    public Permission RequiredUserChannelPermissions { get; }
    public Func<object, object[], Task> InvokeAsync { get; }

    public CommandInfo(MethodInfo methodInfo, CommandAttribute attribute, CommandServiceOptions<TContext> options)
    {
        if (methodInfo.ReturnType != typeof(Task))
            throw new InvalidCommandDefinitionException($"Commands must return {typeof(Task).FullName} | {methodInfo.DeclaringType!.FullName}.{methodInfo.Name}");

        Priority = attribute.Priority;
        RequiredContext = attribute.RequiredContext;
        DeclaringType = methodInfo.DeclaringType!;

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
                throw new InvalidCommandDefinitionException($"Optional parameters must appear after all required parameters | {methodInfo.DeclaringType!.FullName}.{methodInfo.Name}");
            CommandParameters[i] = new(parameter, options);
        }

        InvokeAsync = (obj, parameters) => (Task)methodInfo.Invoke(obj, BindingFlags.DoNotWrapExceptions, null, parameters, null)!;

        RequiredBotPermissions = attribute.RequiredBotPermissions;
        RequiredBotChannelPermissions = attribute.RequiredBotChannelPermissions;
        RequiredUserPermissions = attribute.RequiredUserPermissions;
        RequiredUserChannelPermissions = attribute.RequiredUserChannelPermissions;
    }
}