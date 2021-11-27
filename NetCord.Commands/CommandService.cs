using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace NetCord.Commands;

public class CommandService : CommandService<CommandContext>
{
    public CommandService(CommandServiceOptions<CommandContext> options = null) : base(options)
    {
    }
}

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
        var messageContentWithoutPrefix = context.Message.Content[prefixLength..];
        bool ignoreCase = _options.IgnoreCase;
        var separator = _options.ParamSeparator;
        IEnumerable<KeyValuePair<string, List<CommandInfo<TContext>>>> commands;
        lock (_commands)
            commands = _commands.Where(x => messageContentWithoutPrefix.StartsWith(x.Key, ignoreCase, CultureInfo.InvariantCulture));

        if (!commands.Any())
            throw new CommandNotFoundException();

        var c = commands.MaxBy(x => x.Key.Length);
        int commandLength = c.Key.Length;
        var commandInfos = c.Value;
        string baseArguments;
        if (messageContentWithoutPrefix.Length > commandLength)
        {
            // example: command: "wzium" message: "!wziumy"
            if (messageContentWithoutPrefix[commandLength] != separator)
                throw new CommandNotFoundException();
            baseArguments = messageContentWithoutPrefix[(commandLength + 1)..].TrimStart(separator);
        }
        else
            baseArguments = string.Empty;

        object[] parametersToPass = null;
        CommandInfo<TContext> commandInfo = null;
        int maxIndex = commandInfos.Count - 1;

        var everyonePermissions = context.Guild.EveryoneRole.Permissions;
        var guildRoles = context.Guild.Roles.Values;
        var guildUser = context.Guild.Users[context.Client];
        PermissionFlags botPermissions = everyonePermissions;
        PermissionFlags botChannelPermissions;
        foreach (var permission in guildRoles)
        {
            if (guildUser.RolesIds.Contains(permission))
                botPermissions |= permission.Permissions;
        }

        var permissionOverwrites = ((TextGuildChannel)context.Message.Channel).PermissionOverwrites;

        bool botAdministrator = botPermissions.HasFlag(PermissionFlags.Administrator);
        if (!botAdministrator)
        {
            PermissionFlags denied = default;
            PermissionFlags allowed = default;
            foreach (var r in guildUser.RolesIds)
            {
                if (permissionOverwrites.TryGetValue(r, out var permission))
                {
                    denied |= permission.Denied;
                    allowed |= permission.Allowed;
                }
            }
            if (permissionOverwrites.TryGetValue(guildUser.Id, out var p))
            {
                denied |= p.Denied;
                allowed |= p.Allowed;
            }
            botChannelPermissions = (botPermissions & ~denied) | allowed;
        }
        else
            botChannelPermissions = default;

        guildUser = (GuildUser)context.Message.Author;
        PermissionFlags userPermissions = everyonePermissions;
        PermissionFlags userChannelPermissions;
        foreach (var permission in guildRoles)
        {
            if (guildUser.RolesIds.Contains(permission))
                userPermissions |= permission.Permissions;
        }

        var userAdministrator = userPermissions.HasFlag(PermissionFlags.Administrator);
        if (!userAdministrator)
        {
            PermissionFlags denied = default;
            PermissionFlags allowed = default;
            foreach (var r in guildUser.RolesIds)
            {
                if (permissionOverwrites.TryGetValue(r, out var permission))
                {
                    denied |= permission.Denied;
                    allowed |= permission.Allowed;
                }
            }
            if (permissionOverwrites.TryGetValue(guildUser.Id, out var p))
            {
                denied |= p.Denied;
                allowed |= p.Allowed;
            }
            userChannelPermissions = (userPermissions & ~denied) | allowed;
        }
        else
            userChannelPermissions = default;

        for (int i = 0; i <= maxIndex; i++)
        {
            commandInfo = commandInfos[i];
            bool lastCommand = i == maxIndex;

            if (!botAdministrator)
            {
                if (!botPermissions.HasFlag(commandInfo.RequiredBotPermissions))
                {
                    if (lastCommand)
                    {
                        var missingPermissions = commandInfo.RequiredBotPermissions & ~botPermissions;
                        throw new PermissionException("Required bot permissions: " + missingPermissions, missingPermissions);
                    }
                    else
                        continue;
                }
                if (!botChannelPermissions.HasFlag(commandInfo.RequiredBotChannelPermissions))
                {
                    if (lastCommand)
                    {
                        var missingPermissions = commandInfo.RequiredBotChannelPermissions & ~botChannelPermissions;
                        throw new PermissionException("Required bot channel permissions: " + missingPermissions, missingPermissions);
                    }
                    else
                        continue;
                }
            }
            if (!userAdministrator)
            {
                if (!userPermissions.HasFlag(commandInfo.RequiredUserPermissions))
                {
                    if (lastCommand)
                    {
                        var missingPermissions = commandInfo.RequiredUserPermissions & ~userPermissions;
                        throw new PermissionException("Required user permissions: " + missingPermissions, missingPermissions);
                    }
                    else
                        continue;
                }
                if (!userChannelPermissions.HasFlag(commandInfo.RequiredUserChannelPermissions))
                {
                    if (lastCommand)
                    {
                        var missingPermissions = commandInfo.RequiredUserChannelPermissions & ~userChannelPermissions;
                        throw new PermissionException("Required user channel permissions: " + missingPermissions, missingPermissions);
                    }
                    else
                        continue;
                }
            }

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
                                else if (lastCommand)
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
                                else if (lastCommand)
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
                        else if (lastCommand)
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
                            if (lastCommand)
                                throw;
                            else
                                goto Continue;
                        }
                    }
                    else
                    {
                        if (parameter.HasDefaultValue)
                            parametersToPass[commandParamIndex] = parameter.DefaultValue;
                        else if (lastCommand)
                            throw new ParameterCountException("Too few parameters");
                        else
                            goto Continue;
                    }
                }
                commandParamIndex++;
                bool IsLastParameter() => commandParamIndex == maxCommandParamIndex;
            }
            if (arguments.Length != 0)
            {
                if (lastCommand)
                    throw new ParameterCountException("Too many parameters");
                else
                    continue;
            }
            break;
            Continue:;
        }
        var methodClass = (BaseCommandModule<TContext>)Activator.CreateInstance(commandInfo.DeclaringType);
        methodClass.Context = context;

        await commandInfo.InvokeAsync(methodClass, parametersToPass).ConfigureAwait(false);
    }
}