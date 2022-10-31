using System.Reflection;

using NetCord.Gateway;

namespace NetCord.Services.Interactions;

public class InteractionService<TContext> : IService where TContext : InteractionContext
{
    private readonly InteractionServiceOptions<TContext> _options;
    private readonly Dictionary<string, InteractionInfo<TContext>> _interactions = new();

    public IReadOnlyDictionary<string, InteractionInfo<TContext>> Interactions
    {
        get
        {
            lock (_interactions)
                return _interactions.ToDictionary(p => p.Key, p => p.Value);
        }
    }

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
            throw new InvalidOperationException($"Modules must inherit from '{nameof(BaseInteractionModule<TContext>)}'.");

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
            InteractionInfo<TContext> interactionInfo = new(method, _options);
            _interactions.Add(interactionAttribute.CustomId, interactionInfo);
        }
    }

    public async Task ExecuteAsync(TContext context)
    {
        var separator = _options.ParamSeparator;
        var content = ((ICustomIdInteractionData)context.Interaction.Data).CustomId;
        var index = content.IndexOf(separator);
        string? customId;
        ReadOnlyMemory<char> arguments;
        if (index == -1)
        {
            customId = content;
            arguments = default;
        }
        else
        {
            customId = content[..index];
            arguments = content.AsMemory(index + 1);
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
                ReadOnlyMemory<char> currentArg;
                if (commandParamIndex == maxCommandParamIndex)
                    currentArg = arguments;
                else
                {
                    index = arguments.Span.IndexOf(separator);
                    currentArg = index == -1 ? arguments : arguments[..index];
                    arguments = arguments[(currentArg.Length + 1)..];
                }
                var value = await parameter.TypeReader.ReadAsync(currentArg, context, parameter, _options).ConfigureAwait(false);
                await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                parametersToPass[commandParamIndex] = value;
            }
            else
            {
                var args = new string(arguments.Span).Split(separator, StringSplitOptions.RemoveEmptyEntries);
                var len = args.Length;
                var o = Array.CreateInstance(parameter.Type, len);

                for (var a = 0; a < len; a++)
                {
                    var value = await parameter.TypeReader.ReadAsync(args[a].AsMemory(), context, parameter, _options).ConfigureAwait(false);
                    await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                    o.SetValue(value, a);
                }

                parametersToPass[commandParamIndex] = o;
            }
        }

        var methodClass = (BaseInteractionModule<TContext>)Activator.CreateInstance(interactionInfo.DeclaringType)!;
        methodClass.Context = context;
        await interactionInfo.InvokeAsync(methodClass, parametersToPass!).ConfigureAwait(false);
    }
}
