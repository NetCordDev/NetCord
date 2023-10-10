using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services.Commands;

public partial class CommandService<TContext> where TContext : ICommandContext
{
    private readonly CommandServiceConfiguration<TContext> _configuration;
    private readonly char[] _parameterSeparators;
    private readonly Dictionary<ReadOnlyMemory<char>, SortedList<CommandInfo<TContext>>> _commands;

    public IReadOnlyDictionary<ReadOnlyMemory<char>, IReadOnlyList<CommandInfo<TContext>>> GetCommands()
        => new Dictionary<ReadOnlyMemory<char>, IReadOnlyList<CommandInfo<TContext>>>(_commands.Select(c => new KeyValuePair<ReadOnlyMemory<char>, IReadOnlyList<CommandInfo<TContext>>>(c.Key, c.Value.ToArray())));

    public CommandService(CommandServiceConfiguration<TContext>? configuration = null)
    {
        _configuration = configuration ??= new();
        _parameterSeparators = configuration.ParameterSeparators.ToArray();
        _commands = new(configuration.IgnoreCase ? ReadOnlyMemoryCharComparer.InvariantCultureIgnoreCase : ReadOnlyMemoryCharComparer.InvariantCulture);
    }

    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly)
    {
        var baseType = typeof(BaseCommandModule<TContext>);
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAssignableTo(baseType))
                AddModuleCore(type);
        }
    }

    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseCommandModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from '{nameof(BaseCommandModule<TContext>)}'.");

        AddModuleCore(type);
    }

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] T>()
    {
        AddModule(typeof(T));
    }

    private void AddModuleCore([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type)
    {
        var configuration = _configuration;
        ReadOnlySpan<char> separators = _parameterSeparators;

        foreach (var method in type.GetMethods())
        {
            var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
            if (commandAttribute is null)
                continue;
            CommandInfo<TContext> commandInfo = new(method, type, commandAttribute, configuration);
            foreach (var alias in commandAttribute.Aliases)
            {
                var aliasMemory = alias.AsMemory();

                if (aliasMemory.Span.IndexOfAny(separators) >= 0)
                    throw new InvalidDefinitionException($"Any alias cannot contain '{nameof(_configuration.ParameterSeparators)}'.", method);

                if (!_commands.TryGetValue(aliasMemory, out var list))
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
                    _commands.Add(aliasMemory, list);
                }
                list.Add(commandInfo);
            }
        }
    }

    public async Task ExecuteAsync(int prefixLength, TContext context, IServiceProvider? serviceProvider = null)
    {
        var fullCommand = context.Message.Content.AsMemory(prefixLength);
        var separators = _parameterSeparators;
        var index = fullCommand.Span.IndexOfAny(separators);
        SortedList<CommandInfo<TContext>> commandInfos;
        ReadOnlyMemory<char> baseArguments;
        if (index == -1)
        {
            var command = fullCommand;
            commandInfos = GetCommandInfos(command);
            baseArguments = default;
        }
        else
        {
            var command = fullCommand[..index];
            commandInfos = GetCommandInfos(command);
            baseArguments = fullCommand[(index + 1)..];
        }
        var configuration = _configuration;

        var maxIndex = commandInfos.Count - 1;

        for (var i = 0; i <= maxIndex; i++)
        {
            var commandInfo = commandInfos[i];
            var lastCommand = i == maxIndex;

            try
            {
                await commandInfo.EnsureCanExecuteAsync(context, serviceProvider).ConfigureAwait(false);
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
                            value = await parameter.TypeReader.ReadAsync(currentArg, context, parameter, configuration, serviceProvider).ConfigureAwait(false);
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
                            await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
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
                        await ReadParamsAsync(context, separators, parametersToPass, arguments, paramIndex, parameter, configuration, serviceProvider).ConfigureAwait(false);
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
            await commandInfo.InvokeAsync(parametersToPass, context, serviceProvider).ConfigureAwait(false);
            break;
            NextCommand:;
        }
    }

    private SortedList<CommandInfo<TContext>> GetCommandInfos(ReadOnlyMemory<char> command)
    {
        if (_commands.TryGetValue(command, out var commandInfos))
            return commandInfos;

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

    [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode", Justification = "The type of the array is known to be present")]
    private static async Task ReadParamsAsync(TContext context, char[] separators, object?[] parametersToPass, ReadOnlyMemory<char> arguments, int paramIndex, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var ranges = Split(arguments.Span, separators);
        var count = ranges.Count;
        var array = Array.CreateInstance(parameter.ElementType, count);
        for (int i = 0; i < count; i++)
        {
            var value = await parameter.TypeReader.ReadAsync(arguments[ranges[i]], context, parameter, configuration, serviceProvider).ConfigureAwait(false);
            await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
            array.SetValue(value, i);
        }
        parametersToPass[paramIndex] = array;

        static List<Range> Split(ReadOnlySpan<char> arguments, ReadOnlySpan<char> separators)
        {
            List<Range> result = [];

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
