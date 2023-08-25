using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandService<TContext, TAutocompleteContext> : ApplicationCommandService<TContext> where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    public ApplicationCommandService(ApplicationCommandServiceConfiguration<TContext>? configuration = null) : base(configuration)
    {
    }

    public async Task ExecuteAutocompleteAsync(TAutocompleteContext context, IServiceProvider? serviceProvider = null)
    {
        var interaction = context.Interaction;
        var data = interaction.Data;
        var command = (IAutocompleteInfo)GetApplicationCommandInfo(data.Id);
        var choices = await command.InvokeAutocompleteAsync(context, data.Options, serviceProvider).ConfigureAwait(false);
        await interaction.SendResponseAsync(InteractionCallback.ApplicationCommandAutocompleteResult(choices)).ConfigureAwait(false);
    }

    private protected override void OnAutocompleteAdd(IAutocompleteInfo autocompleteInfo)
    {
        autocompleteInfo.InitializeAutocomplete<TAutocompleteContext>();
    }
}

public class ApplicationCommandService<TContext> where TContext : IApplicationCommandContext
{
    private protected readonly ApplicationCommandServiceConfiguration<TContext> _configuration;
    private protected readonly Dictionary<ulong, ApplicationCommandInfo<TContext>> _commands = new();

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

    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly)
    {
        var baseType = typeof(BaseApplicationCommandModule<TContext>);
        lock (_commands)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(baseType))
                    AddModuleCore(type);
            }
        }
    }

    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseApplicationCommandModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from '{nameof(ApplicationCommandModule<TContext>)}'.");

        lock (_commands)
            AddModuleCore(type);
    }

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] T>()
    {
        AddModule(typeof(T));
    }

    private void AddModuleCore([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type)
    {
        var configuration = _configuration;

        var slashCommandAttribute = type.GetCustomAttribute<SlashCommandAttribute>();
        if (slashCommandAttribute is not null)
        {
            SlashCommandGroupInfo<TContext> slashCommandGroupInfo = new(type, slashCommandAttribute, configuration);
            OnAutocompleteAdd(slashCommandGroupInfo);
            AddCommandInfo(slashCommandGroupInfo);
        }
        else
        {
            foreach (var method in type.GetMethods())
            {
                slashCommandAttribute = method.GetCustomAttribute<SlashCommandAttribute>();
                if (slashCommandAttribute is not null)
                {
                    SlashCommandInfo<TContext> slashCommandInfo = new(method, type, slashCommandAttribute, configuration);
                    OnAutocompleteAdd(slashCommandInfo);
                    AddCommandInfo(slashCommandInfo);
                }

                var userCommandAttribute = method.GetCustomAttribute<UserCommandAttribute>();
                if (userCommandAttribute is not null)
                    AddCommandInfo(new UserCommandInfo<TContext>(method, type, userCommandAttribute, configuration));

                var messageCommandAttribute = method.GetCustomAttribute<MessageCommandAttribute>();
                if (messageCommandAttribute is not null)
                    AddCommandInfo(new MessageCommandInfo<TContext>(method, type, messageCommandAttribute, configuration));
            }
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
        List<ApplicationCommand> list = new(includeGuildCommands ? _globalCommandsToCreate.Count + _guildCommandsToCreate.Count : _globalCommandsToCreate.Count);
        var e = (await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, _globalCommandsToCreate.Select(c => c.GetRawValue()), properties).ConfigureAwait(false)).Zip(_globalCommandsToCreate);
        lock (_commands)
        {
            foreach (var (first, second) in e)
            {
                _commands[first.Key] = second;
                list.Add(first.Value);
            }
        }

        if (includeGuildCommands)
        {
            foreach (var c in _guildCommandsToCreate.GroupBy(c => c.GuildId.GetValueOrDefault()))
            {
                var rawGuildCommands = c.Select(v => v.GetRawValue());

                var newCommands = await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, c.Key, rawGuildCommands, properties).ConfigureAwait(false);
                lock (_commands)
                {
                    foreach (var (first, second) in newCommands.Zip(c))
                    {
                        _commands[first.Key] = second;
                        list.Add(first.Value);
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

    internal void AddCommands(IReadOnlyList<(ApplicationCommand Command, IApplicationCommandInfo CommandInfo)> commands)
    {
        lock (_commands)
        {
            int count = commands.Count;
            for (int i = 0; i < count; i++)
            {
                var (command, commandInfo) = commands[i];
                _commands[command.Id] = (ApplicationCommandInfo<TContext>)commandInfo;
            }
        }
    }

    internal void AddCommand((ApplicationCommand Command, IApplicationCommandInfo CommandInfo) command)
    {
        lock (_commands)
            _commands[command.Command.Id] = (ApplicationCommandInfo<TContext>)command.CommandInfo;
    }

    public Task ExecuteAsync(TContext context, IServiceProvider? serviceProvider = null)
    {
        var interaction = context.Interaction;
        var command = GetApplicationCommandInfo(interaction.Data.Id);

        return command.InvokeAsync(context, _configuration, serviceProvider);
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

    private protected virtual void OnAutocompleteAdd(IAutocompleteInfo autocompleteInfo)
    {
    }
}
