using System.Collections.ObjectModel;
using System.Reflection;

namespace NetCord.Services.Commands;

public record CommandInfo<TContext> where TContext : ICommandContext
{
    public Type DeclaringType { get; }
    public ReadOnlyCollection<CommandParameter<TContext>> Parameters { get; }
    public int Priority { get; }
    public Permission RequiredBotPermissions { get; }
    public Permission RequiredBotChannelPermissions { get; }
    public Permission RequiredUserPermissions { get; }
    public Permission RequiredUserChannelPermissions { get; }
    public Func<object, object[], Task> InvokeAsync { get; }

    public CommandInfo(MethodInfo methodInfo, CommandAttribute attribute, CommandServiceOptions<TContext> options)
    {
        if (methodInfo.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Commands must return {typeof(Task).FullName}", methodInfo);

        Priority = attribute.Priority;
        DeclaringType = methodInfo.DeclaringType!;

        var parameters = methodInfo.GetParameters();
        var parametersLength = parameters.Length;
        var p = new CommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException($"Optional parameters must appear after all required parameters", methodInfo);
            p[i] = new(parameter, options);
        }
        Parameters = new(p);

        InvokeAsync = (obj, parameters) => (Task)methodInfo.Invoke(obj, BindingFlags.DoNotWrapExceptions, null, parameters, null)!;

        CommandModuleAttribute? moduleAttribute = DeclaringType.GetCustomAttribute<CommandModuleAttribute>();
        if (moduleAttribute != null)
        {
            RequiredBotPermissions = attribute.RequiredBotPermissions | moduleAttribute.RequiredBotPermissions;
            RequiredBotChannelPermissions = attribute.RequiredBotChannelPermissions | moduleAttribute.RequiredBotChannelPermissions;
            RequiredUserPermissions = attribute.RequiredUserPermissions | moduleAttribute.RequiredUserPermissions;
            RequiredUserChannelPermissions = attribute.RequiredUserChannelPermissions | moduleAttribute.RequiredUserChannelPermissions;
        }
        else
        {
            RequiredBotPermissions = attribute.RequiredBotPermissions;
            RequiredBotChannelPermissions = attribute.RequiredBotChannelPermissions;
            RequiredUserPermissions = attribute.RequiredUserPermissions;
            RequiredUserChannelPermissions = attribute.RequiredUserChannelPermissions;
        }
    }
}