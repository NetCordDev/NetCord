using System.Reflection;

namespace NetCord.Services.SlashCommands;

public class SlashCommandService<TContext> : IService where TContext : BaseSlashCommandContext
{
    private readonly SlashCommandServiceOptions<TContext> _options;
    private readonly Dictionary<string, SlashCommandInfo<TContext>> _commands = new();

    public SlashCommandService(SlashCommandServiceOptions<TContext>? options = null)
    {
        _options = options ?? new();
    }

    public void AddModules(Assembly assembly)
    {
        Type baseType = typeof(SlashCommandModule<TContext>);
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
        if (!type.IsAssignableTo(typeof(SlashCommandModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from {nameof(SlashCommandModule<TContext>)}");

        lock (_commands)
            AddModuleCore(type);
    }

    private void AddModuleCore(Type type)
    {
        foreach (var method in type.GetMethods())
        {
            SlashCommandAttribute? interactionAttribute = method.GetCustomAttribute<SlashCommandAttribute>();
            if (interactionAttribute == null)
                continue;
            SlashCommandInfo<TContext> interactionInfo = new(method, interactionAttribute, _options);
            _commands.Add(interactionAttribute.Name, interactionInfo);
        }
    }

    public async Task CreateCommandsAsync(RestClient client, DiscordId applicationId, bool includeGuildCommands = false)
    {
        var commands = _commands.Values;
        if (includeGuildCommands)
        {
            List<ApplicationCommandProperties> globalCommands = new();
            Dictionary<DiscordId, List<(ApplicationCommandProperties Command, IEnumerable<ApplicationCommandPermissionProperties> Permissions)>> guildsCommands = new();
            foreach (var c in commands)
            {
                if (c.GuildId.HasValue)
                {
                    var guildId = c.GuildId.GetValueOrDefault();
                    if (guildsCommands.TryGetValue(guildId, out var commandList))
                        commandList.Add((c.GetRawValue(), c.GetRawPermissions()));
                    else
                    {
                        commandList = new()
                        {
                            (c.GetRawValue(), c.GetRawPermissions())
                        };
                        guildsCommands.Add(guildId, commandList);
                    }
                }
                else
                {
                    globalCommands.Add(c.GetRawValue());
                }
            }

            await client.Interaction.ApplicationCommand.BulkOverwriteGlobalAsync(applicationId, globalCommands).ConfigureAwait(false);

            foreach (var c in guildsCommands)
            {
                var newCommands = await client.Interaction.ApplicationCommand.BulkOverwriteGuildAsync(applicationId, c.Key, c.Value.Select(v => v.Command)).ConfigureAwait(false);
                await client.Interaction.ApplicationCommand.BulkOverwriteApplicationCommandPermissions(applicationId, c.Key, newCommands.Zip(c.Value.Select(v => v.Permissions)).Select(z => new GuildApplicationCommandPermissionsProperties(z.First.Key, z.Second))).ConfigureAwait(false);
            }
        }
        else
        {
            await client.Interaction.ApplicationCommand.BulkOverwriteGlobalAsync(applicationId, commands.Where(c => !c.GuildId.HasValue).Select(c => c.GetRawValue())).ConfigureAwait(false);
        }
    }

    public async Task ExecuteAsync(TContext context)
    {
        var interaction = context.Interaction;
        SlashCommandInfo<TContext> command;
        lock (_commands)
        {
            if (!_commands.TryGetValue(interaction.Data.Name, out command!))
                throw new SlashCommandNotFoundException();
        }

        var guild = context.Interaction.Guild;
        if (guild != null)
        {
            if (context.Client.User == null)
                throw new NullReferenceException($"{nameof(context)}.{nameof(context.Client)}.{nameof(context.Client.User)} cannot be null");

            var everyonePermissions = guild.EveryoneRole.Permissions;
            var channelPermissionOverwrites = guild.Channels[context.Interaction.ChannelId.GetValueOrDefault()].PermissionOverwrites;
            var roles = guild.Roles.Values;

            UserHelper.CalculatePermissions(guild.Users[context.Client.User],
                guild,
                everyonePermissions,
                channelPermissionOverwrites,
                roles,
                out Permission botPermissions,
                out Permission botChannelPermissions,
                out var botAdministrator);
            UserHelper.CalculatePermissions((GuildUser)context.Interaction.User,
                guild,
                everyonePermissions,
                channelPermissionOverwrites,
                roles,
                out Permission userPermissions,
                out Permission userChannelPermissions,
                out var userAdministrator);

            #region Checking Permissions
            if (!botAdministrator)
            {
                if (!botPermissions.HasFlag(command.RequiredBotPermissions))
                {
                    var missingPermissions = command.RequiredBotPermissions & ~botPermissions;
                    throw new PermissionException("Required bot permissions: " + missingPermissions, missingPermissions);
                }
                if (!botChannelPermissions.HasFlag(command.RequiredBotChannelPermissions))
                {
                    var missingPermissions = command.RequiredBotChannelPermissions & ~botChannelPermissions;
                    throw new PermissionException("Required bot channel permissions: " + missingPermissions, missingPermissions);
                }
            }
            if (!userAdministrator)
            {
                if (!userPermissions.HasFlag(command.RequiredUserPermissions))
                {
                    var missingPermissions = command.RequiredUserPermissions & ~userPermissions;
                    throw new PermissionException("Required user permissions: " + missingPermissions, missingPermissions);
                }
                if (!userChannelPermissions.HasFlag(command.RequiredUserChannelPermissions))
                {
                    var missingPermissions = command.RequiredUserChannelPermissions & ~userChannelPermissions;
                    throw new PermissionException("Required user channel permissions: " + missingPermissions, missingPermissions);
                }
            }
            #endregion
        }

        var parameters = command.Parameters;
        var values = new object[parameters.Count];
        var options = interaction.Data.Options;
        int optionsCount = options.Count;
        int parameterIndex = 0;
        for (int optionIndex = 0; optionIndex < optionsCount; parameterIndex++)
        {
            var parameter = parameters[parameterIndex];
            var option = options[optionIndex];
            if (parameter.Name != option.Name)
                values[parameterIndex] = parameter.DefaultValue!;
            else
            {
                values[parameterIndex] = await parameter.TypeReader.ReadAsync(option.Value!, context, parameter, _options).ConfigureAwait(false);
                optionIndex++;
            }
        }
        int parametersCount = parameters.Count;
        while (parameterIndex < parametersCount)
        {
            values[parameterIndex] = parameters[parameterIndex].DefaultValue!;
            parameterIndex++;
        }

        var methodClass = (SlashCommandModule<TContext>)Activator.CreateInstance(command.DeclaringType)!;
        methodClass.Context = context;
        await command.InvokeAsync(methodClass, values).ConfigureAwait(false);
    }

    public async Task ExecuteAutocompleteAsync(ApplicationCommandAutocompleteInteraction autocompleteInteraction)
    {
        var parameter = autocompleteInteraction.Data.Options.First(o => o.Focused);
        SlashCommandInfo<TContext> command;
        lock (_commands)
        {
            if (!_commands.TryGetValue(autocompleteInteraction.Data.Name, out command!))
                throw new SlashCommandNotFoundException();
        }
        var autocompletes = command.Autocompletes;
        Autocomplete autocomplete;
        lock (autocompletes)
        {
            if (!autocompletes.TryGetValue(parameter.Name, out autocomplete!))
                throw new AutocompleteNotFoundException();
        }
        var choices = await autocomplete(parameter, autocompleteInteraction).ConfigureAwait(false);
        await autocompleteInteraction.SendResponseAsync(InteractionCallback.ApplicationCommandAutocompleteResult(choices)).ConfigureAwait(false);
    }
}