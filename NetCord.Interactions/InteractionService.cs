using System.Reflection;

namespace NetCord.Interactions;
public class InteractionService : InteractionService<ButtonInteractionContext, MenuInteractionContext>
{
}

public class InteractionService<TButtonInteractionContext, UMenuInteractionContext> where TButtonInteractionContext : IButtonInteractionContext where UMenuInteractionContext : IMenuInteractionContext
{
    private readonly Dictionary<string, InteractionInfo> _actionButtonInteractions = new();
    private readonly Dictionary<string, InteractionInfo> _menuInteractions = new();

    public void AddModules(Assembly assembly)
    {
        Type[] types = assembly.GetTypes();
        AddButtonModules(types);
        AddMenuModules(types);
    }

    public void AddMenuModules(Assembly assembly)
    {
        AddMenuModules(assembly.GetTypes());
    }

    public void AddActionButtonModules(Assembly assembly)
    {
        AddButtonModules(assembly.GetTypes());
    }

    private void AddButtonModules(Type[] types)
    {
        IEnumerable<MethodInfo[]> methodsEnumerable = types.Where(x => x.IsAssignableTo(typeof(ButtonInteractionModule<TButtonInteractionContext>))).Select(x => x.GetMethods());
        foreach (MethodInfo[] methods in methodsEnumerable)
        {
            foreach (MethodInfo method in methods)
            {
                InteractionAttribute? interactionAttribute = method.GetCustomAttribute<InteractionAttribute>();
                if (interactionAttribute == null)
                    continue;
                if (method.GetParameters().Length != 0)
                    throw new TargetParameterCountException("Interactions must be parameterless");
                InteractionInfo interactionInfo = new(method);
                _actionButtonInteractions.Add(interactionAttribute.CustomId, interactionInfo);
            }
        }
    }

    private void AddMenuModules(Type[] types)
    {
        IEnumerable<MethodInfo[]> methodsEnumerable = types.Where(x => x.IsAssignableTo(typeof(MenuInteractionModule<UMenuInteractionContext>))).Select(x => x.GetMethods());
        foreach (MethodInfo[] methods in methodsEnumerable)
        {
            foreach (MethodInfo method in methods)
            {
                InteractionAttribute? interactionAttribute = method.GetCustomAttribute<InteractionAttribute>();
                if (interactionAttribute == null)
                    continue;
                if (method.GetParameters().Length != 0)
                    throw new TargetParameterCountException("Interactions must be parameterless");
                InteractionInfo interactionInfo = new(method);
                _menuInteractions.Add(interactionAttribute.CustomId, interactionInfo);
            }
        }
    }

    public async Task ExecuteAsync(TButtonInteractionContext context)
    {
        if (_actionButtonInteractions.TryGetValue(context.Interaction.Data.CustomId, out var interactionInfo))
        {
            var methodClass = (ButtonInteractionModule<TButtonInteractionContext>)Activator.CreateInstance(interactionInfo.DeclaringType)!;
            methodClass.Context = context;
            await interactionInfo.InvokeAsync(methodClass).ConfigureAwait(false);
        }
        else
            throw new InteractionNotFoundException();
    }

    public Task ExecuteAsync(UMenuInteractionContext context)
    {
        if (_menuInteractions.TryGetValue(context.Interaction.Data.CustomId, out var interactionInfo))
        {
            var methodClass = (MenuInteractionModule<UMenuInteractionContext>)Activator.CreateInstance(interactionInfo.DeclaringType)!;
            methodClass.Context = context;
            return interactionInfo.InvokeAsync(methodClass);
        }
        else
            throw new InteractionNotFoundException();
    }
}