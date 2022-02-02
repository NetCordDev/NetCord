using System.Collections.ObjectModel;
using System.Reflection;

namespace NetCord.Services.Commands;

public class CommandService : CommandService<CommandContext>
{
    public CommandService(CommandServiceOptions<CommandContext>? options = null) : base(options)
    {
    }
}

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

        var guild = context.Guild;

        ValueTuple<CommandInfo<TContext>, object[]> v;
        if (guild != null)
        {
            if (context.Client.User == null)
                throw new NullReferenceException($"{nameof(context)}.{nameof(context.Client)}.{nameof(context.Client.User)} cannot be null");

            var everyonePermissions = guild.EveryoneRole.Permissions;
            var channelPermissionOverwrites = ((IGuildChannel)context.Message.Channel).PermissionOverwrites;
            var roles = context.Guild!.Roles.Values;

            UserHelper.CalculatePermissions(guild.Users[context.Client.User],
                guild,
                everyonePermissions,
                channelPermissionOverwrites,
                roles,
                out Permission botPermissions,
                out Permission botChannelPermissions,
                out var botAdministrator);
            UserHelper.CalculatePermissions((GuildUser)context.Message.Author,
                guild,
                everyonePermissions,
                channelPermissionOverwrites,
                roles,
                out Permission userPermissions,
                out Permission userChannelPermissions,
                out var userAdministrator);

            v = await GetMethodAndParametersWithPermissionCheckAsync(context, separators, commandInfos, baseArguments, maxIndex, botPermissions, botChannelPermissions, userPermissions, userChannelPermissions, botAdministrator, userAdministrator).ConfigureAwait(false);
        }
        else
            v = await GetMethodAndParametersAsync(context, separators, commandInfos, baseArguments, maxIndex).ConfigureAwait(false);

        var methodClass = (BaseCommandModule<TContext>)Activator.CreateInstance(v.Item1.DeclaringType)!;
        methodClass.Context = context;

        await v.Item1.InvokeAsync(methodClass, v.Item2).ConfigureAwait(false);
    }

    private async Task<(CommandInfo<TContext> commandInfo, object[] parametersToPass)> GetMethodAndParametersWithPermissionCheckAsync(TContext context, char[] separators, SortedList<CommandInfo<TContext>> commandInfos, string baseArguments, int maxIndex, Permission botPermissions, Permission botChannelPermissions, Permission userPermissions, Permission userChannelPermissions, bool botAdministrator, bool userAdministrator)
    {
        CommandInfo<TContext>? commandInfo = null;
        object?[]? parametersToPass = null;
        for (var i = 0; i <= maxIndex; i++)
        {
            commandInfo = commandInfos[i];
            var lastCommand = i == maxIndex;

            #region Checking Permissions
            if (!botAdministrator)
            {
                if (!botPermissions.HasFlag(commandInfo.RequiredBotPermissions))
                    if (lastCommand)
                    {
                        var missingPermissions = commandInfo.RequiredBotPermissions & ~botPermissions;
                        throw new PermissionException("Required bot permissions: " + missingPermissions, missingPermissions);
                    }
                    else
                        continue;
                if (!botChannelPermissions.HasFlag(commandInfo.RequiredBotChannelPermissions))
                    if (lastCommand)
                    {
                        var missingPermissions = commandInfo.RequiredBotChannelPermissions & ~botChannelPermissions;
                        throw new PermissionException("Required bot channel permissions: " + missingPermissions, missingPermissions);
                    }
                    else
                        continue;
            }
            if (!userAdministrator)
            {
                if (!userPermissions.HasFlag(commandInfo.RequiredUserPermissions))
                    if (lastCommand)
                    {
                        var missingPermissions = commandInfo.RequiredUserPermissions & ~userPermissions;
                        throw new PermissionException("Required user permissions: " + missingPermissions, missingPermissions);
                    }
                    else
                        continue;
                if (!userChannelPermissions.HasFlag(commandInfo.RequiredUserChannelPermissions))
                    if (lastCommand)
                    {
                        var missingPermissions = commandInfo.RequiredUserChannelPermissions & ~userChannelPermissions;
                        throw new PermissionException("Required user channel permissions: " + missingPermissions, missingPermissions);
                    }
                    else
                        continue;
            }
            #endregion

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
            Continue:;
        }
        return (commandInfo, parametersToPass)!;
    }



    private async Task<(CommandInfo<TContext> commandInfo, object[] parametersToPass)> GetMethodAndParametersAsync(TContext context, char[] separators, SortedList<CommandInfo<TContext>> commandInfos, string baseArguments, int maxIndex)
    {
        CommandInfo<TContext>? commandInfo = null;
        object?[]? parametersToPass = null;
        for (var i = 0; i <= maxIndex; i++)
        {
            commandInfo = commandInfos[i];
            var lastCommand = i == maxIndex;

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
                    continue;
                commandParamIndex++;
            }
            if (arguments.Length != 0)
                if (lastCommand)
                    throw new ParameterCountException("Too many parameters");
                else
                    goto Continue;
            Break:
            break;
            Continue:;
        }
        return (commandInfo, parametersToPass)!;
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