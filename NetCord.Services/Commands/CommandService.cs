using System.Reflection;

namespace NetCord.Services.Commands;

public partial class CommandService<TContext> : IService where TContext : ICommandContext
{
    private readonly CommandServiceOptions<TContext> _options;
    private readonly char[] _paramSeparators;
    private readonly Dictionary<string, SortedList<CommandInfo<TContext>>> _commands;

    public IReadOnlyDictionary<string, IReadOnlyList<CommandInfo<TContext>>> Commands
    {
        get
        {
            lock (_commands)
                return _commands.ToDictionary(v => v.Key, v => (IReadOnlyList<CommandInfo<TContext>>)v.Value);
        }
    }

    public CommandService(CommandServiceOptions<TContext>? options = null)
    {
        _options = options ?? new();
        _paramSeparators = _options.ParamSeparators.ToArray();
        _commands = new(_options.IgnoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture);
    }

    public void AddModules(Assembly assembly)
    {
        Type baseType = typeof(BaseCommandModule<TContext>);
        lock (_commands)
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
        if (!type.IsAssignableTo(typeof(BaseCommandModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from '{nameof(BaseCommandModule<TContext>)}'.");

        lock (_commands)
            AddModuleCore(type);
    }

    private void AddModuleCore(Type type)
    {
        foreach (var method in type.GetMethods())
        {
            CommandAttribute? commandAttribute = method.GetCustomAttribute<CommandAttribute>();
            if (commandAttribute == null)
                continue;
            CommandInfo<TContext> commandInfo = new(method, commandAttribute, _options);
            foreach (var alias in commandAttribute.Aliases)
            {
                if (alias.ContainsAny(_paramSeparators))
                    throw new InvalidDefinitionException($"Any alias cannot contain '{nameof(_options.ParamSeparators)}'.", method);
                if (!_commands.TryGetValue(alias, out var list))
                {
                    list = new((ci1, ci2) =>
                    {
                        var ci1Priority = ci1.Priority;
                        var ci2Priority = ci2.Priority;
                        if (ci1Priority > ci2Priority)
                            return -1;
                        if (ci1Priority < ci2Priority)
                            return 1;

                        var ci1CommandParametersLength = ci1.Parameters.Count;
                        var ci2CommandParametersLength = ci2.Parameters.Count;
                        if (ci1CommandParametersLength > ci2CommandParametersLength)
                            return -1;
                        if (ci1CommandParametersLength < ci2CommandParametersLength)
                            return 1;
                        return 0;
                    });
                    _commands.Add(alias, list);
                }
                list.Add(commandInfo);
            }
        }
    }

    public async Task ExecuteAsync(int prefixLength, TContext context)
    {
        var messageContentWithoutPrefix = context.Message.Content[prefixLength..];
        var separators = _paramSeparators;

        SortedList<CommandInfo<TContext>> commandInfos;
        ReadOnlyMemory<char> baseArguments;
        var index = messageContentWithoutPrefix.IndexOfAny(separators);
        if (index == -1)
        {
            lock (_commands)
                if (!_commands.TryGetValue(messageContentWithoutPrefix, out commandInfos!))
                    throw new CommandNotFoundException();
            baseArguments = default;
        }
        else
        {
            lock (_commands)
                if (!_commands.TryGetValue(messageContentWithoutPrefix[..index], out commandInfos!))
                    throw new CommandNotFoundException();
            baseArguments = messageContentWithoutPrefix.AsMemory(index + 1).TrimStart(separators);
        }

        var maxIndex = commandInfos.Count - 1;

        for (var i = 0; i <= maxIndex; i++)
        {
            var commandInfo = commandInfos[i];
            var lastCommand = i == maxIndex;

            try
            {
                await commandInfo.EnsureCanExecuteAsync(context).ConfigureAwait(false);
            }
            catch
            {
                if (lastCommand)
                    throw;
                else
                    continue;
            }

            var commandParameters = commandInfo.Parameters;
            var commandParametersLength = commandParameters.Count;
            var arguments = baseArguments;
            var isLastArgGood = false;
            ReadOnlyMemory<char> currentArg = default;

            bool isStatic = commandInfo.Static;
            object?[] values;
            ArraySegment<object?> parametersToPass;
            if (isStatic)
            {
                values = new object?[commandParametersLength];
                parametersToPass = values;
            }
            else
            {
                values = new object?[commandParametersLength + 1];
                parametersToPass = new(values, 1, commandParametersLength);
            }

            int paramIndex = 0;
            var maxParamIndex = commandParametersLength - 1;
            while (paramIndex <= maxParamIndex)
            {
                CommandParameter<TContext> parameter = commandParameters[paramIndex];

                if (!parameter.Params)
                {
                    UpdateCurrentArg(separators, arguments, isLastArgGood, parameter.Remainder, ref currentArg);

                    var currentArgLength = currentArg.Length;
                    if (currentArgLength != 0)
                    {
                        try
                        {
                            var value = await parameter.TypeReader.ReadAsync(currentArg, context, parameter, _options).ConfigureAwait(false);
                            await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                            parametersToPass[paramIndex] = value;
                            arguments = arguments[currentArgLength..].TrimStart(separators);
                            isLastArgGood = false;
                        }
                        catch
                        {
                            // is not last parameter
                            if (paramIndex != maxParamIndex && parameter.HasDefaultValue)
                            {
                                var value = parameter.DefaultValue;
                                await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                                parametersToPass[paramIndex] = value;
                                isLastArgGood = true;
                            }
                            else if (lastCommand)
                                throw;
                            else
                                goto Continue;
                        }
                    }
                    else if (parameter.HasDefaultValue)
                    {
                        var value = parameter.DefaultValue;
                        await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                        parametersToPass[paramIndex] = value;
                        isLastArgGood = true;
                    }
                    else if (lastCommand)
                        throw new ParameterCountException("Too few parameters.");
                    else
                        goto Continue;
                }
                else if (arguments.Length != 0)
                {
                    try
                    {
                        await ReadParamsAsync(context, separators, parametersToPass, arguments, paramIndex, parameter).ConfigureAwait(false);
                        goto Break;
                    }
                    catch
                    {
                        if (lastCommand)
                            throw;
                        else
                            goto Continue;
                    }
                }
                else if (parameter.HasDefaultValue)
                {
                    var value = parameter.DefaultValue;
                    await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                    parametersToPass[paramIndex] = value;
                }
                else if (lastCommand)
                    throw new ParameterCountException("Too few parameters.");
                else
                    goto Continue;
                paramIndex++;
            }
            if (arguments.Length != 0)
                if (lastCommand)
                    throw new ParameterCountException("Too many parameters.");
                else
                    continue;
            Break:
            if (!isStatic)
            {
                var methodClass = (BaseCommandModule<TContext>)Activator.CreateInstance(commandInfo.DeclaringType)!;
                methodClass.Context = context;
                values[0] = methodClass;
            }
            await commandInfo.InvokeAsync(values).ConfigureAwait(false);
            break;
            Continue:;
        }
    }

    private static void UpdateCurrentArg(char[] separators, ReadOnlyMemory<char> arguments, bool isLastArgGood, bool remainder, ref ReadOnlyMemory<char> currentArg)
    {
        if (remainder)
            currentArg = arguments;
        else if (isLastArgGood == false)
        {
            var index = arguments.Span.IndexOfAny(separators);
            currentArg = index == -1 ? arguments : arguments[..index];
        }
    }

    private async Task ReadParamsAsync(TContext context, char[] separators, ArraySegment<object?> parametersToPass, ReadOnlyMemory<char> arguments, int paramIndex, CommandParameter<TContext> parameter)
    {
        var args = new string(arguments.Span).Split(separators, StringSplitOptions.RemoveEmptyEntries);
        var len = args.Length;
        var o = Array.CreateInstance(parameter.Type, len);

        for (var a = 0; a < len; a++)
        {
            var value = await parameter.TypeReader.ReadAsync(args[a].AsMemory(), context, parameter, _options).ConfigureAwait(false);
            await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
            o.SetValue(value, a);
        }

        parametersToPass[paramIndex] = o;
    }
}
