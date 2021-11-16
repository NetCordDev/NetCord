using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace NetCord.Commands;

public class CommandService<TContext> where TContext : ICommandContext
{
    private readonly CommandServiceOptions<TContext> _options;
    private readonly Dictionary<string, List<CommandInfo<TContext>>> _commands = new();

    public IReadOnlyDictionary<string, ReadOnlyCollection<CommandInfo<TContext>>> Commands
    {
        get
        {
            lock (_commands)
                return _commands.ToDictionary(v => v.Key, v => v.Value.AsReadOnly());
        }
    }

    public CommandService(CommandServiceOptions<TContext> options = null)
    {
        _options = options ?? new();
    }

    public void AddModules(Assembly assembly)
    {
        Type baseType = typeof(BaseCommandModule<TContext>);
        IEnumerable<MethodInfo[]> methodsIEnumerable = assembly.GetTypes().Where(x => x.IsAssignableTo(baseType)).Select(x => x.GetMethods());
        lock (_commands)
        {
            foreach (MethodInfo[] methods in methodsIEnumerable)
            {
                foreach (MethodInfo method in methods)
                {
                    CommandAttribute commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                    if (commandAttribute == null)
                        continue;
                    CommandInfo<TContext> commandInfo = new(method, commandAttribute, _options.TypeReaders);
                    foreach (var alias in commandAttribute.Aliases)
                    {
                        if (!_commands.TryGetValue(alias, out var list))
                        {
                            list = new();
                            _commands.Add(alias, list);
                        }
                        list.Add(commandInfo);
                        list.Sort((ci1, ci2) =>
                        {
                            int ci1Priority = ci1.Priority;
                            int ci2Priority = ci2.Priority;
                            if (ci1Priority > ci2Priority)
                                return -1;
                            if (ci1Priority < ci2Priority)
                                return 1;

                            int ci1CommandParametersLength = ci1.CommandParameters.Length;
                            int ci2CommandParametersLength = ci2.CommandParameters.Length;
                            if (ci1CommandParametersLength > ci2CommandParametersLength)
                                return -1;
                            if (ci1CommandParametersLength < ci2CommandParametersLength)
                                return 1;
                            return 0;
                        });
                    }
                }
            }
        }
    }

    public async Task ExecuteAsync(int prefixLength, TContext context)
    {
        var commandContent = context.Message.Content[prefixLength..];
        bool ignoreCase = _options.IgnoreCase;
        var separator = _options.ParamSeparator;
        IEnumerable<KeyValuePair<string, List<CommandInfo<TContext>>>> commands;
        lock (_commands)
            commands = _commands.Where(x => commandContent.StartsWith(x.Key, ignoreCase, CultureInfo.InvariantCulture));

        if (!commands.Any())
            throw new CommandNotFoundException();

        var c = commands.MaxBy(x => x.Key.Length);
        int commandLength = c.Key.Length;
        var commandInfos = c.Value;
        string baseArguments;
        if (commandContent.Length > commandLength)
        {
            // example: command: "wzium" message: "!wziumy"
            if (commandContent[commandLength] != separator)
                throw new CommandNotFoundException();
            baseArguments = commandContent[(commandLength + 1)..].TrimStart(separator);
        }
        else
            baseArguments = string.Empty;

        object[] parametersToPass = null;
        CommandInfo<TContext> commandInfo = null;
        int maxIndex = commandInfos.Count - 1;

        for (int i = 0; i <= maxIndex; i++)
        {
            commandInfo = commandInfos[i];

            string arguments = baseArguments;

            CommandParameter<TContext>[] commandParameters = commandInfo.CommandParameters;
            var commandParametersLength = commandParameters.Length;
            parametersToPass = new object[commandParametersLength];
            int commandParamIndex = 0;
            var isLastArgGood = false;
            string currentArg = null;

            int maxCommandParamIndex = commandParametersLength - 1;

            while (commandParamIndex <= maxCommandParamIndex)
            {
                CommandParameter<TContext> parameter = commandParameters[commandParamIndex];

                if (!parameter.Params)
                {
                    if (parameter.Remainder)
                        currentArg = arguments;
                    else if (isLastArgGood == false)
                    {
                        int index = arguments.IndexOf(separator);
                        currentArg = index == -1 ? arguments : arguments[..index];
                    }

                    int currentArgLength = currentArg.Length;
                    if (currentArgLength != 0)
                    {
                        if (!parameter.EnumTypeReader)
                        {
                            try
                            {
                                parametersToPass[commandParamIndex] = await parameter.ReadAsync(currentArg, context, _options).ConfigureAwait(false);
                                arguments = arguments[currentArgLength..].TrimStart(separator);
                                isLastArgGood = false;
                            }
                            catch
                            {
                                if (!IsLastParameter() && parameter.HasDefaultValue)
                                {
                                    parametersToPass[commandParamIndex] = parameter.DefaultValue;
                                    isLastArgGood = true;
                                }
                                else if (IsLastCommand())
                                    throw;
                                else
                                    goto Continue;
                            }
                        }
                        else
                        {
                            try
                            {
                                parametersToPass[commandParamIndex] = await _options.EnumTypeReader.Invoke(currentArg, parameter.Type, context, _options).ConfigureAwait(false);
                                arguments = arguments[currentArgLength..].TrimStart(separator);
                                isLastArgGood = false;
                            }
                            catch
                            {
                                if (!IsLastParameter() && parameter.HasDefaultValue)
                                {
                                    parametersToPass[commandParamIndex] = parameter.DefaultValue;
                                    isLastArgGood = true;
                                }
                                else if (IsLastCommand())
                                    throw;
                                else
                                    goto Continue;
                            }
                        }
                    }
                    else
                    {
                        if (parameter.HasDefaultValue)
                        {
                            parametersToPass[commandParamIndex] = parameter.DefaultValue;
                            isLastArgGood = true;
                        }
                        else if (IsLastCommand())
                            throw new ParameterCountException("Too few parameters");
                        else
                            goto Continue;
                    }
                }
                else
                {
                    if (arguments.Length != 0)
                    {
                        try
                        {
                            var args = arguments.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            var len = args.Length;
                            var parameterType = parameter.Type;
                            var o = Array.CreateInstance(parameterType, len);
                            if (!parameter.EnumTypeReader)
                            {
                                for (var a = 0; a < len; a++)
                                    o.SetValue(await parameter.ReadAsync(args[a], context, _options).ConfigureAwait(false), a);
                            }
                            else
                            {
                                for (var a = 0; a < len; a++)
                                    o.SetValue(await _options.EnumTypeReader.Invoke(args[a], parameterType, context, _options).ConfigureAwait(false), a);
                            }

                            parametersToPass[commandParamIndex] = o;
                            arguments = string.Empty;
                        }
                        catch
                        {
                            if (IsLastCommand())
                                throw;
                            else
                                goto Continue;
                        }
                    }
                    else
                    {
                        if (parameter.HasDefaultValue)
                            parametersToPass[commandParamIndex] = parameter.DefaultValue;
                        else if (IsLastCommand())
                            throw new ParameterCountException("Too few parameters");
                        else
                            goto Continue;
                    }
                }
                commandParamIndex++;
            }
            if (arguments.Length != 0)
            {
                if (IsLastCommand())
                    throw new ParameterCountException("Too many parameters");
                else
                    continue;
            }
            break;
            Continue:

            bool IsLastCommand() => i == maxIndex;
            bool IsLastParameter() => commandParamIndex == maxCommandParamIndex;
        }
        var methodClass = (BaseCommandModule<TContext>)Activator.CreateInstance(commandInfo.DeclaringType);
        methodClass.Context = context;

        await commandInfo.InvokeAsync(methodClass, parametersToPass).ConfigureAwait(false);
    }
}