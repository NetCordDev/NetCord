using System.Reflection;
using System.Runtime.CompilerServices;

using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandService<TContext, TAutocompleteContext> : ApplicationCommandService<TContext> where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    public ApplicationCommandService(ApplicationCommandServiceConfiguration<TContext>? configuration = null) : base(configuration)
    {
        _supportsAutocomplete = true;
        _autocompleteBase = typeof(IAutocompleteProvider<TAutocompleteContext>);
    }

    public async Task ExecuteAutocompleteAsync(TAutocompleteContext context)
    {
        var interaction = context.Interaction;
        ApplicationCommandInfo<TContext> command = GetApplicationCommandInfo(interaction.Data.Id);
        var autocompletes = command.Autocompletes!;
        var option = interaction.Data.Options.First(o => o.Focused);
        if (!autocompletes.TryGetValue(option.Name, out var autocompleteProvider))
            throw new AutocompleteNotFoundException();
        var choices = await Unsafe.As<IAutocompleteProvider<TAutocompleteContext>>(autocompleteProvider).GetChoicesAsync(option, context).ConfigureAwait(false);
        await interaction.SendResponseAsync(InteractionCallback.ApplicationCommandAutocompleteResult(choices)).ConfigureAwait(false);
    }
}

public class ApplicationCommandService<TContext> : IService where TContext : IApplicationCommandContext
{
    private protected readonly ApplicationCommandServiceConfiguration<TContext> _configuration;
    private protected readonly Dictionary<ulong, ApplicationCommandInfo<TContext>> _commands = new();
    private protected bool _supportsAutocomplete;
    private protected Type? _autocompleteBase;

    internal readonly List<ApplicationCommandInfo<TContext>> _globalCommandsToCreate = new();
    internal readonly List<ApplicationCommandInfo<TContext>> _guildCommandsToCreate = new();

    public IReadOnlyDictionary<ulong, ApplicationCommandInfo<TContext>> GetCommands()
    {
        lock (_commands)
            return new Dictionary<ulong, ApplicationCommandInfo<TContext>>(_commands);
    }

    public ApplicationCommandService(ApplicationCommandServiceConfiguration<TContext>? configuration = null)
    {
        _configuration = configuration ?? new();
    }

    public void AddModules(Assembly assembly)
    {
        Type baseType = typeof(ApplicationCommandModule<TContext>);
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
        if (!type.IsAssignableTo(typeof(ApplicationCommandModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from '{nameof(ApplicationCommandModule<TContext>)}'.");

        lock (_commands)
            AddModuleCore(type);
    }

    private void AddModuleCore(Type type)
    {
        var configuration = _configuration;
        foreach (var method in type.GetMethods())
        {
            SlashCommandAttribute? slashCommandAttribute = method.GetCustomAttribute<SlashCommandAttribute>();
            if (slashCommandAttribute != null)
                AddCommandInfo(new(method, slashCommandAttribute, configuration, _supportsAutocomplete, _autocompleteBase));

            UserCommandAttribute? userCommandAttribute = method.GetCustomAttribute<UserCommandAttribute>();
            if (userCommandAttribute != null)
                AddCommandInfo(new(method, userCommandAttribute, configuration));

            MessageCommandAttribute? messageCommandAttribute = method.GetCustomAttribute<MessageCommandAttribute>();
            if (messageCommandAttribute != null)
                AddCommandInfo(new(method, messageCommandAttribute, configuration));
        }

        void AddCommandInfo(ApplicationCommandInfo<TContext> applicationCommandInfo)
        {
            if (applicationCommandInfo.GuildId.HasValue)
                _guildCommandsToCreate.Add(applicationCommandInfo);
            else
                _globalCommandsToCreate.Add(applicationCommandInfo);
        }
    }

    public async Task<IReadOnlyList<ApplicationCommand>> CreateCommandsAsync(RestClient client, ulong applicationId, bool includeGuildCommands = false, RequestProperties? properties = null)
    {
        List<ApplicationCommand> list = new(_globalCommandsToCreate.Count + _guildCommandsToCreate.Count);
        var e = (await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, _globalCommandsToCreate.Select(c => c.GetRawValue()), properties).ConfigureAwait(false)).Zip(_globalCommandsToCreate);
        lock (_commands)
        {
            foreach (var (First, Second) in e)
            {
                _commands[First.Key] = Second;
                list.Add(First.Value);
            }
        }

        if (includeGuildCommands)
        {
            foreach (var c in _guildCommandsToCreate.GroupBy(c => c.GuildId))
            {
                var rawGuildCommands = c.Select(v => v.GetRawValue());

                var newCommands = await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, c.Key.GetValueOrDefault(), rawGuildCommands, properties).ConfigureAwait(false);
                lock (_commands)
                {
                    foreach (var (First, Second) in newCommands.Zip(c))
                    {
                        _commands[First.Key] = Second;
                        list.Add(First.Value);
                    }
                }
            }
        }
        return list;
    }

    public IEnumerable<ApplicationCommandProperties> GetGlobalCommandProperties()
        => _globalCommandsToCreate.Select(c => c.GetRawValue());

    public IEnumerable<IGrouping<ulong, ApplicationCommandProperties>> GetGuildCommandProperties()
        => _guildCommandsToCreate.GroupBy(c => c.GuildId.GetValueOrDefault(), c => c.GetRawValue());

    internal void AddCommands(IEnumerable<(ApplicationCommand Command, IApplicationCommandInfo CommandInfo)> commands)
    {
        lock (_commands)
        {
            foreach (var (command, commandInfo) in commands)
                _commands[command.Id] = (ApplicationCommandInfo<TContext>)commandInfo;
        }
    }

    internal void AddCommand((ApplicationCommand Command, IApplicationCommandInfo CommandInfo) command)
    {
        lock (_commands)
            _commands[command.Command.Id] = (ApplicationCommandInfo<TContext>)command.CommandInfo;
    }

    public async Task ExecuteAsync(TContext context)
    {
        var interaction = context.Interaction;
        ApplicationCommandInfo<TContext>? command = GetApplicationCommandInfo(interaction.Data.Id);

        await command.EnsureCanExecuteAsync(context).ConfigureAwait(false);

        object?[]? parametersToPass;

        if (interaction is SlashCommandInteraction slashCommandInteraction)
        {
            var configuration = _configuration;
            var parameters = command.Parameters;
            int parametersCount = parameters!.Count;
            parametersToPass = new object?[parametersCount];
            var options = slashCommandInteraction.Data.Options;
            int optionsCount = options.Count;
            int parameterIndex = 0;
            for (int optionIndex = 0; optionIndex < optionsCount; parameterIndex++)
            {
                var parameter = parameters[parameterIndex];
                var option = options[optionIndex];
                object? value;
                if (parameter.Name == option.Name)
                {
                    value = await parameter.TypeReader.ReadAsync(option.Value!, context, parameter, configuration).ConfigureAwait(false);
                    await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                    optionIndex++;
                }
                else
                    value = parameter.DefaultValue;

                parametersToPass[parameterIndex] = value;
            }
            while (parameterIndex < parametersCount)
            {
                var parameter = parameters[parameterIndex];
                parametersToPass[parameterIndex] = parameter.DefaultValue;
                parameterIndex++;
            }
        }
        else
            parametersToPass = null;

        await command.InvokeAsync(parametersToPass, context).ConfigureAwait(false);
    }

    private protected ApplicationCommandInfo<TContext> GetApplicationCommandInfo(ulong commandId)
    {
        ApplicationCommandInfo<TContext>? applicationCommandInfo;
        bool success;
        lock (_commands)
            success = _commands.TryGetValue(commandId, out applicationCommandInfo);

        if (success)
            return applicationCommandInfo!;
        throw new ApplicationCommandNotFoundException();
    }
}
