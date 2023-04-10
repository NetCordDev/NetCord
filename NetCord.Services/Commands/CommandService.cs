using System.Buffers;
using System.Reflection;

namespace NetCord.Services.Commands;

public partial class CommandService<TContext> : IService where TContext : ICommandContext
{
    private readonly CommandServiceConfiguration<TContext> _configuration;
    private readonly char[] _parameterSeparators;
    private readonly Dictionary<string, SortedList<CommandInfo<TContext>>> _commands;

    public IReadOnlyDictionary<string, IReadOnlyList<CommandInfo<TContext>>> GetCommands()
    {
        lock (_commands)
            return new Dictionary<string, IReadOnlyList<CommandInfo<TContext>>>(_commands.Select(c => new KeyValuePair<string, IReadOnlyList<CommandInfo<TContext>>>(c.Key, c.Value.ToArray())));
    }

    public CommandService(CommandServiceConfiguration<TContext>? configuration = null)
    {
        _configuration = configuration ??= new();
        _parameterSeparators = configuration.ParameterSeparators.ToArray();
        _commands = new(configuration.IgnoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture);
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
        var configuration = _configuration;
        foreach (var method in type.GetMethods())
        {
            CommandAttribute? commandAttribute = method.GetCustomAttribute<CommandAttribute>();
            if (commandAttribute == null)
                continue;
            CommandInfo<TContext> commandInfo = new(method, commandAttribute, configuration);
            foreach (var alias in commandAttribute.Aliases)
            {
                if (alias.ContainsAny(_parameterSeparators))
                    throw new InvalidDefinitionException($"Any alias cannot contain '{nameof(_configuration.ParameterSeparators)}'.", method);
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
        var content = context.Message.Content;
        var separators = _parameterSeparators;
        var index = content.IndexOfAny(separators, prefixLength);
        SortedList<CommandInfo<TContext>> commandInfos;
        ReadOnlyMemory<char> baseArguments;
        if (index == -1)
        {
            var command = content[prefixLength..];
            commandInfos = GetCommandInfos(command);
            baseArguments = default;
        }
        else
        {
            var command = content[prefixLength..index];
            commandInfos = GetCommandInfos(command);
            baseArguments = content.AsMemory(index + 1);
        }
        var configuration = _configuration;

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

            var parametersToPass = new object?[commandParametersLength];

            int paramIndex = 0;
            var maxParamIndex = commandParametersLength - 1;
            while (paramIndex <= maxParamIndex)
            {
                var parameter = commandParameters[paramIndex];

                if (!parameter.Params)
                {
                    UpdateCurrentArg(separators, arguments, isLastArgGood, parameter.Remainder, ref currentArg);

                    var currentArgLength = currentArg.Length;
                    if (currentArgLength != 0)
                    {
                        object? value;
                        try
                        {
                            value = await parameter.TypeReader.ReadAsync(currentArg, context, parameter, configuration).ConfigureAwait(false);
                        }
                        catch
                        {
                            if (parameter.HasDefaultValue && paramIndex != maxParamIndex)
                            {
                                parametersToPass[paramIndex] = parameter.DefaultValue;
                                isLastArgGood = true;
                                goto Skip;
                            }
                            else if (lastCommand)
                                throw;
                            else
                                goto NextCommand;
                        }
                        try
                        {
                            await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                        }
                        catch
                        {
                            if (lastCommand)
                                throw;
                            else
                                goto NextCommand;
                        }
                        parametersToPass[paramIndex] = value;
                        arguments = arguments[currentArgLength..].TrimStart(separators);
                        isLastArgGood = false;
                        Skip:;
                    }
                    else if (parameter.HasDefaultValue)
                    {
                        parametersToPass[paramIndex] = parameter.DefaultValue;
                        isLastArgGood = true;
                    }
                    else if (lastCommand)
                        throw new ParameterCountException(ParameterCountExceptionType.TooFew);
                    else
                        goto NextCommand;
                }
                else if (!arguments.IsEmpty)
                {
                    try
                    {
                        await ReadParamsAsync(context, separators, parametersToPass, arguments, paramIndex, parameter, configuration).ConfigureAwait(false);
                    }
                    catch
                    {
                        if (lastCommand)
                            throw;
                        else
                            goto NextCommand;
                    }
                    goto Break;
                }
                else if (parameter.HasDefaultValue)
                {
                    parametersToPass[paramIndex] = parameter.DefaultValue;
                    goto Break;
                }
                else if (lastCommand)
                    throw new ParameterCountException(ParameterCountExceptionType.TooFew);
                else
                    goto NextCommand;
                paramIndex++;
            }
            if (arguments.Length != 0)
            {
                if (lastCommand)
                    throw new ParameterCountException(ParameterCountExceptionType.TooMany);
                else
                    continue;
            }
            Break:
            await commandInfo.InvokeAsync(parametersToPass, context).ConfigureAwait(false);
            break;
            NextCommand:;
        }
    }

    private SortedList<CommandInfo<TContext>> GetCommandInfos(string command)
    {
        SortedList<CommandInfo<TContext>>? commandInfos;
        bool success;
        lock (_commands)
            success = _commands.TryGetValue(command, out commandInfos);

        if (success)
            return commandInfos!;
        throw new CommandNotFoundException();
    }

    private static void UpdateCurrentArg(char[] separators, ReadOnlyMemory<char> arguments, bool isLastArgGood, bool remainder, ref ReadOnlyMemory<char> currentArg)
    {
        if (remainder)
            currentArg = arguments;
        else if (!isLastArgGood)
        {
            var index = arguments.Span.IndexOfAny(separators);
            currentArg = index == -1 ? arguments : arguments[..index];
        }
    }

    private static async Task ReadParamsAsync(TContext context, char[] separators, object?[] parametersToPass, ReadOnlyMemory<char> arguments, int paramIndex, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration)
    {
        var ranges = Split(arguments.Span, separators);
        var count = ranges.Count;
        var array = Array.CreateInstance(parameter.ElementType, count);
        for (int i = 0; i < count; i++)
        {
            var value = await parameter.TypeReader.ReadAsync(arguments[ranges[i]], context, parameter, configuration).ConfigureAwait(false);
            await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
            array.SetValue(value, i);
        }
        parametersToPass[paramIndex] = array;

        static List<Range> Split(ReadOnlySpan<char> arguments, ReadOnlySpan<char> separators)
        {
            List<Range> result = new();

            int startIndex = 0;
            int index;

            while ((index = arguments.IndexOfAny(separators)) != -1)
            {
                result.Add(new(startIndex, startIndex + index));
                var indexPlusOne = index + 1;
                var move = indexPlusOne + IndexOfNot(arguments[indexPlusOne..], separators);
                startIndex += move;
                arguments = arguments[move..];
            }
            if (!arguments.IsEmpty)
                result.Add(new(startIndex, startIndex + arguments.Length));

            return result;

            static int IndexOfNot(ReadOnlySpan<char> arguments, ReadOnlySpan<char> separators)
            {
                var argumentsLength = arguments.Length;
                var separatorsLength = separators.Length;

                int start = 0;
                while (start < argumentsLength)
                {
                    for (int i = 0; i < separatorsLength; i++)
                    {
                        if (arguments[start] == separators[i])
                            goto Next;
                    }

                    break;
                    Next:
                    start++;
                }

                return start;
            }
        }
    }
}
