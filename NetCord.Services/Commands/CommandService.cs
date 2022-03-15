using System.Collections.ObjectModel;
using System.Reflection;

namespace NetCord.Services.Commands;

public partial class CommandService<TContext> : IService where TContext : ICommandContext
{
    private readonly CommandServiceOptions<TContext> _options;
    private readonly Dictionary<string, SortedList<CommandInfo<TContext>>> _commands;

    public IReadOnlyDictionary<string, ReadOnlyCollection<CommandInfo<TContext>>> Commands
    {
        get
        {
            lock (_commands)
                return _commands.ToDictionary(v => v.Key, v => v.Value.AsReadOnly());
        }
    }

    public CommandService(CommandServiceOptions<TContext>? options = null)
    {
        if (options == null)
        {
            _options = new();
            _commands = new(StringComparer.InvariantCultureIgnoreCase);
        }
        else
        {
            _options = options;
            _commands = new(_options.IgnoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture);
        }
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
            throw new InvalidOperationException($"Modules must inherit from {nameof(BaseCommandModule<TContext>)}");

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
                if (alias.ContainsAny(_options._paramSeparators))
                    throw new InvalidDefinitionException($"Any alias cannot contain {nameof(_options.ParamSeparators)}", method);
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
        var ignoreCase = _options.IgnoreCase;
        var separators = _options._paramSeparators;

        SortedList<CommandInfo<TContext>> commandInfos;
        string baseArguments;
        var index = messageContentWithoutPrefix.IndexOfAny(separators);
        if (index == -1)
        {
            lock (_commands)
                if (!_commands.TryGetValue(messageContentWithoutPrefix, out commandInfos!))
                    throw new CommandNotFoundException();
            baseArguments = string.Empty;
        }
        else
        {
            lock (_commands)
                if (!_commands.TryGetValue(messageContentWithoutPrefix[..index], out commandInfos!))
                    throw new CommandNotFoundException();
            baseArguments = messageContentWithoutPrefix[(index + 1)..].TrimStart(separators);
        }

        var maxIndex = commandInfos.Count - 1;

        CommandInfo<TContext>? commandInfo = null;
        object?[]? parametersToPass = null;
        for (var i = 0; i <= maxIndex; i++)
        {
            commandInfo = commandInfos[i];
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

            ReadOnlyCollection<CommandParameter<TContext>> commandParameters = commandInfo.Parameters;
            var commandParametersLength = commandParameters.Count;
            var arguments = baseArguments;
            parametersToPass = new object[commandParametersLength];
            var isLastArgGood = false;
            string? currentArg = null;

            var commandParamIndex = 0;
            var maxCommandParamIndex = commandParametersLength - 1;
            while (commandParamIndex <= maxCommandParamIndex)
            {
                CommandParameter<TContext> parameter = commandParameters[commandParamIndex];

                if (!parameter.Params)
                {
                    UpdateCurrentArg(separators, arguments, isLastArgGood, parameter.Remainder, ref currentArg);

                    var currentArgLength = currentArg!.Length;
                    if (currentArgLength != 0)
                    {
                        try
                        {
                            parametersToPass[commandParamIndex] = await parameter.TypeReader.ReadAsync(currentArg, context, parameter, _options).ConfigureAwait(false);
                            arguments = arguments[currentArgLength..].TrimStart(separators);
                            isLastArgGood = false;
                        }
                        catch
                        {
                            // is not last parameter
                            if (commandParamIndex != maxCommandParamIndex && parameter.HasDefaultValue)
                            {
                                parametersToPass[commandParamIndex] = parameter.DefaultValue;
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
                        parametersToPass[commandParamIndex] = parameter.DefaultValue;
                        isLastArgGood = true;
                    }
                    else if (lastCommand)
                        throw new ParameterCountException("Too few parameters");
                    else
                        goto Continue;
                }
                else if (arguments.Length != 0)
                {
                    try
                    {
                        await ReadParamsAsync(context, separators, parametersToPass, arguments, commandParamIndex, parameter).ConfigureAwait(false);
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
                    parametersToPass[commandParamIndex] = parameter.DefaultValue;
                else if (lastCommand)
                    throw new ParameterCountException("Too few parameters");
                else
                    goto Continue;
                commandParamIndex++;
            }
            if (arguments.Length != 0)
                if (lastCommand)
                    throw new ParameterCountException("Too many parameters");
                else
                    continue;
            Break:
            break;
            Continue:
            ;
        }

        var methodClass = (BaseCommandModule<TContext>)Activator.CreateInstance(commandInfo!.DeclaringType)!;
        methodClass.Context = context;

        await commandInfo.InvokeAsync(methodClass, parametersToPass!).ConfigureAwait(false);
    }

    private static void UpdateCurrentArg(char[] separators, string arguments, bool isLastArgGood, bool remainder, ref string? currentArg)
    {
        if (remainder)
            currentArg = arguments;
        else if (isLastArgGood == false)
        {
            var index = arguments.IndexOfAny(separators);
            currentArg = index == -1 ? arguments : arguments[..index];
        }
    }

    private async Task ReadParamsAsync(TContext context, char[] separators, object?[] parametersToPass, string arguments, int commandParamIndex, CommandParameter<TContext> parameter)
    {
        var args = arguments.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        var len = args.Length;
        var o = Array.CreateInstance(parameter.Type, len);

        for (var a = 0; a < len; a++)
            o.SetValue(await parameter.TypeReader.ReadAsync(args[a], context, parameter, _options).ConfigureAwait(false), a);

        parametersToPass[commandParamIndex] = o;
    }
}