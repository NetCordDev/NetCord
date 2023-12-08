using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandService<TContext, TAutocompleteContext> : ApplicationCommandService<TContext> where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    public ApplicationCommandService(ApplicationCommandServiceConfiguration<TContext>? configuration = null) : base(configuration)
    {
    }

    public async ValueTask ExecuteAutocompleteAsync(TAutocompleteContext context, IServiceProvider? serviceProvider = null)
    {
        var interaction = context.Interaction;
        var data = interaction.Data;
        var command = (IAutocompleteInfo)GetApplicationCommandInfo(data.Id);
        var choices = await command.InvokeAutocompleteAsync(context, data.Options, serviceProvider).ConfigureAwait(false);
        await interaction.SendResponseAsync(InteractionCallback.Autocomplete(choices)).ConfigureAwait(false);
    }

    private protected override void OnAutocompleteAdd(IAutocompleteInfo autocompleteInfo)
    {
        autocompleteInfo.InitializeAutocomplete<TAutocompleteContext>();
    }
}

public class ApplicationCommandService<TContext> : IApplicationCommandService, IService where TContext : IApplicationCommandContext
{
    private protected readonly ApplicationCommandServiceConfiguration<TContext> _configuration;
    private protected readonly Dictionary<ulong, ApplicationCommandInfo<TContext>> _commands = [];

    internal readonly List<ApplicationCommandInfo<TContext>> _globalCommandsToCreate = [];
    internal readonly Dictionary<ulong, List<ApplicationCommandInfo<TContext>>> _guildCommandsToCreate = [];

    IReadOnlyList<IApplicationCommandInfo> IApplicationCommandService.GlobalCommands => _globalCommandsToCreate;

    IEnumerable<GuildCommands> IApplicationCommandService.GuildCommands => _guildCommandsToCreate.Select(c => new GuildCommands(c.Key, c.Value));

    public IReadOnlyDictionary<ulong, ApplicationCommandInfo<TContext>> GetCommands() => new Dictionary<ulong, ApplicationCommandInfo<TContext>>(_commands);

    public ApplicationCommandService(ApplicationCommandServiceConfiguration<TContext>? configuration = null)
    {
        _configuration = configuration ?? ApplicationCommandServiceConfiguration<TContext>.Default;
    }

    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly)
    {
        var baseType = typeof(BaseApplicationCommandModule<TContext>);
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAssignableTo(baseType))
                AddModuleCore(type);
        }
    }

    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseApplicationCommandModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from '{nameof(ApplicationCommandModule<TContext>)}'.");

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
    }

    public void AddSlashCommand(string name,
                                string description,
                                Delegate handler,
                                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type? nameTranslationsProviderType = null,
                                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type? descriptionTranslationsProviderType = null,
                                Permissions? defaultGuildUserPermissions = null,
                                bool? dMPermission = null,
                                bool defaultPermission = true,
                                bool nsfw = false,
                                ulong? guildId = null)
    {
        var slashCommandInfo = new SlashCommandInfo<TContext>(name,
                                                              description,
                                                              handler,
                                                              nameTranslationsProviderType,
                                                              descriptionTranslationsProviderType,
                                                              defaultGuildUserPermissions,
                                                              dMPermission,
                                                              defaultPermission,
                                                              nsfw,
                                                              guildId,
                                                              _configuration);
        OnAutocompleteAdd(slashCommandInfo);
        AddCommandInfo(slashCommandInfo);
    }

    public void AddUserCommand(string name,
                               Delegate handler,
                               [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type? nameTranslationsProviderType = null,
                               Permissions? defaultGuildUserPermissions = null,
                               bool? dMPermission = null,
                               bool defaultPermission = true,
                               bool nsfw = false,
                               ulong? guildId = null)
    {
        AddCommandInfo(new UserCommandInfo<TContext>(name,
                                                     handler,
                                                     nameTranslationsProviderType,
                                                     defaultGuildUserPermissions,
                                                     dMPermission,
                                                     defaultPermission,
                                                     nsfw,
                                                     guildId,
                                                     _configuration));
    }

    public void AddMessageCommand(string name,
                                  Delegate handler,
                                  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type? nameTranslationsProviderType = null,
                                  Permissions? defaultGuildUserPermissions = null,
                                  bool? dMPermission = null,
                                  bool defaultPermission = true,
                                  bool nsfw = false,
                                  ulong? guildId = null)
    {
        AddCommandInfo(new MessageCommandInfo<TContext>(name,
                                                        handler,
                                                        nameTranslationsProviderType,
                                                        defaultGuildUserPermissions,
                                                        dMPermission,
                                                        defaultPermission,
                                                        nsfw,
                                                        guildId,
                                                        _configuration));
    }

    void IApplicationCommandService.AddCommand(ulong id, IApplicationCommandInfo applicationCommandInfo)
    {
        _commands[id] = (ApplicationCommandInfo<TContext>)applicationCommandInfo;
    }

    private void AddCommandInfo(ApplicationCommandInfo<TContext> applicationCommandInfo)
    {
        if (applicationCommandInfo.GuildId.HasValue)
        {
            var guildCommandsToCreate = _guildCommandsToCreate;
            if (!guildCommandsToCreate.TryGetValue(applicationCommandInfo.GuildId.GetValueOrDefault(), out var list))
                guildCommandsToCreate.Add(applicationCommandInfo.GuildId.GetValueOrDefault(), list = []);

            list.Add(applicationCommandInfo);
        }
        else
            _globalCommandsToCreate.Add(applicationCommandInfo);
    }

    public async Task<IReadOnlyList<ApplicationCommand>> CreateCommandsAsync(RestClient client, ulong applicationId, bool includeGuildCommands = false, RequestProperties? properties = null)
    {
        int count = includeGuildCommands ? _globalCommandsToCreate.Count + _guildCommandsToCreate.Count : _globalCommandsToCreate.Count;
        List<ApplicationCommand> list = new(count);

        var globalProperties = _globalCommandsToCreate.Select(c => c.GetRawValue());
        var created = await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, globalProperties, properties).ConfigureAwait(false);

        foreach (var (command, commandInfo) in created.Zip(_globalCommandsToCreate))
        {
            _commands[command.Key] = commandInfo;
            list.Add(command.Value);
        }

        if (includeGuildCommands)
        {
            foreach (var guildCommandsPair in _guildCommandsToCreate)
            {
                var guildCommands = guildCommandsPair.Value;
                var guildProperties = guildCommands.Select(v => v.GetRawValue());

                var guildCreated = await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, guildCommandsPair.Key, guildProperties, properties).ConfigureAwait(false);
                foreach (var (command, commandInfo) in guildCreated.Zip(guildCommands))
                {
                    _commands[command.Key] = commandInfo;
                    list.Add(command.Value);
                }
            }
        }
        return list;
    }

    //internal void AddCommands(IReadOnlyList<(ApplicationCommand Command, IApplicationCommandInfo CommandInfo)> commands)
    //{
    //    int count = commands.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        var (command, commandInfo) = commands[i];
    //        _commands[command.Id] = (ApplicationCommandInfo<TContext>)commandInfo;
    //    }
    //}

    //internal void AddCommand((ApplicationCommand Command, IApplicationCommandInfo CommandInfo) command)
    //{
    //    _commands[command.Command.Id] = (ApplicationCommandInfo<TContext>)command.CommandInfo;
    //}

    public ValueTask ExecuteAsync(TContext context, IServiceProvider? serviceProvider = null)
    {
        var interaction = context.Interaction;
        var command = GetApplicationCommandInfo(interaction.Data.Id);

        return command.InvokeAsync(context, _configuration, serviceProvider);
    }

    private protected ApplicationCommandInfo<TContext> GetApplicationCommandInfo(ulong commandId)
    {
        if (_commands.TryGetValue(commandId, out var applicationCommandInfo))
            return applicationCommandInfo;

        throw new ApplicationCommandNotFoundException();
    }

    private protected virtual void OnAutocompleteAdd(IAutocompleteInfo autocompleteInfo)
    {
    }
}
