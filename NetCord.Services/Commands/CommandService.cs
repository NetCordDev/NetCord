using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Services.Helpers;

namespace NetCord.Services.Commands;

public partial class CommandService<TContext>(CommandServiceConfiguration<TContext>? configuration = null) : ICommandService where TContext : ICommandContext
{
    private readonly CommandServiceConfiguration<TContext> _configuration = configuration ??= CommandServiceConfiguration<TContext>.Default;
    private readonly char[] _parameterSeparators = configuration.ParameterSeparators.ToArray();
    private readonly Dictionary<ReadOnlyMemory<char>, SortedList<CommandInfo<TContext>>> _commands = new(configuration.IgnoreCase ? ReadOnlyMemoryCharComparer.InvariantCultureIgnoreCase : ReadOnlyMemoryCharComparer.InvariantCulture);

    public IReadOnlyDictionary<ReadOnlyMemory<char>, IReadOnlyList<CommandInfo<TContext>>> GetCommands()
        => new Dictionary<ReadOnlyMemory<char>, IReadOnlyList<CommandInfo<TContext>>>(_commands.Select(c => new KeyValuePair<ReadOnlyMemory<char>, IReadOnlyList<CommandInfo<TContext>>>(c.Key, [.. c.Value])));

    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly)
    {
        foreach (var type in ServiceHelpers.GetModules(typeof(BaseCommandModule<TContext>), assembly))
            AddModuleCore(type);
    }

    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseCommandModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from '{typeof(BaseCommandModule<TContext>)}'.");

        AddModuleCore(type);
    }

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] T>()
    {
        AddModule(typeof(T));
    }

    private void AddModuleCore([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type)
    {
        var configuration = _configuration;

        foreach (var method in type.GetMethods())
        {
            var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
            if (commandAttribute is null)
                continue;
            CommandInfo<TContext> commandInfo = new(method, type, commandAttribute.Priority, configuration);
            AddCommandInfo(commandAttribute.Aliases, commandInfo, method);
        }
    }

    public void AddCommand(IEnumerable<string> aliases, Delegate handler, int priority = 0)
    {
        CommandInfo<TContext> commandInfo = new(handler, priority, _configuration);
        AddCommandInfo(aliases, commandInfo, handler.Method);
    }

    private void AddCommandInfo(IEnumerable<string> aliases, CommandInfo<TContext> commandInfo, MethodInfo method)
    {
        foreach (var alias in aliases)
        {
            var aliasMemory = alias.AsMemory();

            if (aliasMemory.Span.IndexOfAny(_parameterSeparators) >= 0)
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

    public async ValueTask<IExecutionResult> ExecuteAsync(int prefixLength, TContext context, IServiceProvider? serviceProvider = null)
    {
        try
        {
            return await ExecuteAsyncCore(prefixLength, context, serviceProvider).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExecutionExceptionResult(ex);
        }
    }

    private async ValueTask<IExecutionResult> ExecuteAsyncCore(int prefixLength, TContext context, IServiceProvider? serviceProvider)
    {
        var fullCommand = context.Message.Content.AsMemory(prefixLength);
        var separators = _parameterSeparators;
        var index = fullCommand.Span.IndexOfAny(separators);
        SortedList<CommandInfo<TContext>>? commandInfos;
        ReadOnlyMemory<char> baseArguments;
        if (index >= 0)
        {
            var command = fullCommand[..index];

            if (!TryGetCommandInfos(command, out commandInfos))
                return new NotFoundResult("Command not found.");

            baseArguments = fullCommand[(index + 1)..].TrimStart(separators);
        }
        else
        {
            var command = fullCommand;

            if (!TryGetCommandInfos(command, out commandInfos))
                return new NotFoundResult("Command not found.");

            baseArguments = default;
        }

        var configuration = _configuration;
        var maxIndex = commandInfos.Count - 1;

        for (var i = 0; i <= maxIndex; i++)
        {
            var commandInfo = commandInfos[i];
            var lastCommand = i == maxIndex;

            var preconditionResult = await commandInfo.EnsureCanExecuteAsync(context, serviceProvider).ConfigureAwait(false);
            if (preconditionResult is IFailResult)
            {
                if (lastCommand)
                    return preconditionResult;
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
                        var typeReaderResult = await parameter.ReadAsync(currentArg, context, configuration, serviceProvider).ConfigureAwait(false);

                        if (typeReaderResult is not TypeReaderSuccessResult typeReaderSuccessResult)
                        {
                            if (parameter.HasDefaultValue && paramIndex != maxParamIndex)
                            {
                                parametersToPass[paramIndex] = parameter.DefaultValue;
                                isLastArgGood = true;
                                goto Skip;
                            }
                            else if (lastCommand)
                                return typeReaderResult;
                            else
                                goto NextCommand;
                        }

                        var value = typeReaderSuccessResult.Value;

                        var parameterPreconditionResult = await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
                        if (parameterPreconditionResult is IFailResult)
                        {
                            if (lastCommand)
                                return parameterPreconditionResult;
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
                        return new ParameterCountMismatchResult(ParameterCountMismatchType.TooFew);
                    else
                        goto NextCommand;
                }
                else if (!arguments.IsEmpty)
                {
                    var result = await ReadParamsAsync(context, separators, parametersToPass, arguments, paramIndex, parameter, configuration, serviceProvider).ConfigureAwait(false);
                    if (result is IFailResult)
                    {
                        if (lastCommand)
                            return result;
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
                    return new ParameterCountMismatchResult(ParameterCountMismatchType.TooFew);
                else
                    goto NextCommand;
                paramIndex++;
            }
            if (arguments.Length != 0)
            {
                if (lastCommand)
                    return new ParameterCountMismatchResult(ParameterCountMismatchType.TooMany);
                else
                    continue;
            }
            Break:
            try
            {
                await commandInfo.InvokeAsync(parametersToPass, context, serviceProvider).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new ExecutionExceptionResult(ex);
            }
            break;
            NextCommand:;
        }

        return SuccessResult.Instance;
    }

    private bool TryGetCommandInfos(ReadOnlyMemory<char> command, [MaybeNullWhen(false)] out SortedList<CommandInfo<TContext>> result)
    {
        return _commands.TryGetValue(command, out result);
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
    private static async ValueTask<IExecutionResult> ReadParamsAsync(TContext context, char[] separators, object?[] parametersToPass, ReadOnlyMemory<char> arguments, int paramIndex, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var ranges = Split(arguments.Span, separators);
        var count = ranges.Count;
        var array = Array.CreateInstance(parameter.ElementType, count);
        for (int i = 0; i < count; i++)
        {
            var typeReaderResult = await parameter.ReadAsync(arguments[ranges[i]], context, configuration, serviceProvider).ConfigureAwait(false);

            if (typeReaderResult is not TypeReaderSuccessResult typeReaderSuccessResult)
                return typeReaderResult;

            var value = typeReaderSuccessResult.Value;

            var preconditionResult = await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);

            if (preconditionResult is IFailResult)
                return preconditionResult;

            array.SetValue(value, i);
        }
        parametersToPass[paramIndex] = array;

        return SuccessResult.Instance;

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
