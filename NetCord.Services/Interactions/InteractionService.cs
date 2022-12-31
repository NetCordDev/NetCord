using System.Reflection;

using NetCord.Gateway;

namespace NetCord.Services.Interactions;

public class InteractionService<TContext> : IService where TContext : InteractionContext
{
    private readonly InteractionServiceConfiguration<TContext> _configuration;
    private readonly Dictionary<string, InteractionInfo<TContext>> _interactions = new();

    public IReadOnlyDictionary<string, InteractionInfo<TContext>> Interactions
    {
        get
        {
            lock (_interactions)
                return _interactions.ToDictionary(p => p.Key, p => p.Value);
        }
    }

    public InteractionService(InteractionServiceConfiguration<TContext>? configuration = null)
    {
        _configuration = configuration ?? new();
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
            InteractionInfo<TContext> interactionInfo = new(method, _configuration);
            _interactions.Add(interactionAttribute.CustomId, interactionInfo);
        }
    }

    public async Task ExecuteAsync(TContext context)
    {
        var separator = _configuration.ParamSeparator;
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
        int interactionParametersLength = interactionParameters.Count;

        bool isStatic = interactionInfo.Static;
        object?[] values;
        ArraySegment<object?> parametersToPass;
        if (isStatic)
        {
            values = new object?[interactionParametersLength];
            parametersToPass = values;
        }
        else
        {
            values = new object?[interactionParametersLength + 1];
            parametersToPass = new(values, 1, interactionParametersLength);
        }

        var maxCommandParamIndex = interactionParametersLength - 1;
        for (int paramIndex = 0; paramIndex <= maxCommandParamIndex; paramIndex++)
        {
            InteractionParameter<TContext> parameter = interactionParameters[paramIndex];
            if (!parameter.Params)
            {
                ReadOnlyMemory<char> currentArg;
                if (paramIndex == maxCommandParamIndex)
                    currentArg = arguments;
                else
                {
                    index = arguments.Span.IndexOf(separator);
                    currentArg = index == -1 ? arguments : arguments[..index];
                    arguments = arguments[(currentArg.Length + 1)..];
                }
                var value = await parameter.TypeReader.ReadAsync(currentArg, context, parameter, _configuration).ConfigureAwait(false);
                await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                parametersToPass[paramIndex] = value;
            }
            else
            {
                var args = new string(arguments.Span).Split(separator, StringSplitOptions.RemoveEmptyEntries);
                var len = args.Length;
                var o = Array.CreateInstance(parameter.Type, len);

                for (var a = 0; a < len; a++)
                {
                    var value = await parameter.TypeReader.ReadAsync(args[a].AsMemory(), context, parameter, _configuration).ConfigureAwait(false);
                    await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                    o.SetValue(value, a);
                }

                parametersToPass[paramIndex] = o;
            }
        }

        if (!isStatic)
        {
            var methodClass = (BaseInteractionModule<TContext>)Activator.CreateInstance(interactionInfo.DeclaringType)!;
            methodClass.Context = context;
            values[0] = methodClass;
        }
        await interactionInfo.InvokeAsync(values).ConfigureAwait(false);
    }
}
