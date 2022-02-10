using System.Collections.ObjectModel;
using System.Reflection;

namespace NetCord.Services.Interactions;

public class InteractionInfo<TContext> where TContext : InteractionContext
{
    public Type DeclaringType { get; }
    public ReadOnlyCollection<InteractionParameter<TContext>> Parameters { get; }
    public int Priority { get; }
    public Permission RequiredBotPermissions { get; }
    public Permission RequiredBotChannelPermissions { get; }
    public Permission RequiredUserPermissions { get; }
    public Permission RequiredUserChannelPermissions { get; }
    public Func<object, object[], Task> InvokeAsync { get; }
    public ReadOnlyCollection<PreconditionAttribute<TContext>> Preconditions { get; }

    public InteractionInfo(MethodInfo methodInfo, InteractionAttribute attribute, InteractionServiceOptions<TContext> options)
    {
        if (methodInfo.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Interactions must return {typeof(Task).FullName}", methodInfo);

        DeclaringType = methodInfo.DeclaringType!;

        var parameters = methodInfo.GetParameters();
        var parametersLength = parameters.Length;
        var p = new InteractionParameter<TContext>[parametersLength];
        for (var i = 0; i < parametersLength; i++)
            p[i] = new(parameters[i], options);
        Parameters = new(p);

        InvokeAsync = (obj, parameters) => (Task)methodInfo.Invoke(obj, BindingFlags.DoNotWrapExceptions, null, parameters, null)!;

        //ModuleAttribute? moduleAttribute = DeclaringType.GetCustomAttribute<ModuleAttribute>();
        //if (moduleAttribute != null)
        //{
        //    RequiredBotPermissions = attribute.RequiredBotPermissions | moduleAttribute.RequiredBotPermissions;
        //    RequiredBotChannelPermissions = attribute.RequiredBotChannelPermissions | moduleAttribute.RequiredBotChannelPermissions;
        //    RequiredUserPermissions = attribute.RequiredUserPermissions | moduleAttribute.RequiredUserPermissions;
        //    RequiredUserChannelPermissions = attribute.RequiredUserChannelPermissions | moduleAttribute.RequiredUserChannelPermissions;
        //}
        //else
        //{
        //    RequiredBotPermissions = attribute.RequiredBotPermissions;
        //    RequiredBotChannelPermissions = attribute.RequiredBotChannelPermissions;
        //    RequiredUserPermissions = attribute.RequiredUserPermissions;
        //    RequiredUserChannelPermissions = attribute.RequiredUserChannelPermissions;
        //}

        Preconditions = new(PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(methodInfo, DeclaringType));
    }

    internal async Task EnsureCanExecuteAsync(TContext context)
    {
        foreach (var preconditionAttribute in Preconditions)
            await preconditionAttribute.EnsureCanExecuteAsync(context).ConfigureAwait(false);
    }
}