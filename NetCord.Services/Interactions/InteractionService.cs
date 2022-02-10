using System.Collections.ObjectModel;
using System.Reflection;

namespace NetCord.Services.Interactions;

public class InteractionService<TContext> : IService where TContext : InteractionContext
{
    private readonly InteractionServiceOptions<TContext> _options;
    private readonly Dictionary<string, InteractionInfo<TContext>> _interactions = new();

    public InteractionService(InteractionServiceOptions<TContext>? options = null)
    {
        _options = options ?? new();
    }

    public void AddModules(Assembly assembly)
    {
        Type baseType = typeof(BaseInteractionModule<TContext>);
        lock (_interactions)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(baseType))
                    AddModuleCore(type);
            }
        }
    }

    public void AddModule(Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseInteractionModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from {nameof(BaseInteractionModule<TContext>)}");

        lock (_interactions)
            AddModuleCore(type);
    }

    private void AddModuleCore(Type type)
    {
        foreach (var method in type.GetMethods())
        {
            InteractionAttribute? interactionAttribute = method.GetCustomAttribute<InteractionAttribute>();
            if (interactionAttribute == null)
                continue;
            InteractionInfo<TContext> interactionInfo = new(method, interactionAttribute, _options);
            _interactions.Add(interactionAttribute.Alias, interactionInfo);
        }
    }

    public async Task ExecuteAsync(TContext context)
    {
        var separator = _options.ParamSeparator;
        var content = ((ButtonInteractionData)context.Interaction.Data).CustomId;
        var index = content.IndexOf(separator);
        string? customId;
        string arguments;
        if (index == -1)
        {
            customId = content;
            arguments = string.Empty;
        }
        else
        {
            customId = content[..index];
            arguments = content[(index + 1)..];
        }
        InteractionInfo<TContext> interactionInfo;
        lock (_interactions)
        {
            if (!_interactions.TryGetValue(customId, out interactionInfo!))
                throw new InteractionNotFoundException();
        }

        await interactionInfo.EnsureCanExecuteAsync(context).ConfigureAwait(false);

        var interactionParameters = interactionInfo.Parameters;
        int commandParametersLength = interactionParameters.Count;
        object?[] parametersToPass = new object?[commandParametersLength];

        var maxCommandParamIndex = commandParametersLength - 1;
        for (int commandParamIndex = 0; commandParamIndex <= maxCommandParamIndex; commandParamIndex++)
        {
            InteractionParameter<TContext> parameter = interactionParameters[commandParamIndex];
            if (!parameter.Params)
            {
                string currentArg;
                if (commandParamIndex == maxCommandParamIndex)
                    currentArg = arguments;
                else
                {
                    index = arguments.IndexOf(separator);
                    currentArg = index == -1 ? arguments : arguments[..index];
                    arguments = arguments[(currentArg.Length + 1)..];
                }
                parametersToPass[commandParamIndex] = await parameter.TypeReader.ReadAsync(currentArg, context, parameter, _options).ConfigureAwait(false);
            }
            else
            {
                var args = arguments.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                var len = args.Length;
                var o = Array.CreateInstance(parameter.Type, len);

                for (var a = 0; a < len; a++)
                    o.SetValue(await parameter.TypeReader.ReadAsync(args[a], context, parameter, _options).ConfigureAwait(false), a);

                parametersToPass[commandParamIndex] = o;
            }
        }

        var methodClass = (BaseInteractionModule<TContext>)Activator.CreateInstance(interactionInfo.DeclaringType)!;
        methodClass.Context = context;
        await interactionInfo.InvokeAsync(methodClass, parametersToPass!).ConfigureAwait(false);
    }
}