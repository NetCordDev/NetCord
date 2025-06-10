using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Represents a service for managing application commands and autocomplete interactions.
/// </summary>
/// <typeparam name="TContext">The application command context.</typeparam>
/// <typeparam name="TAutocompleteContext">The autocomplete interaction context.</typeparam>
/// <param name="configuration"><inheritdoc cref="ApplicationCommandService{TContext}.Configuration" path="/summary" /></param>
public class ApplicationCommandService<TContext, TAutocompleteContext>(ApplicationCommandServiceConfiguration<TContext>? configuration = null) : ApplicationCommandService<TContext>(configuration) where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    /// <summary>
    /// Executes an autocomplete interaction.
    /// </summary>
    /// <param name="context">Context containing the data for the autocomplete interaction execution.</param>
    /// <param name="serviceProvider">Service provider for dependency injection.</param>
    /// <returns>An <see cref="IExecutionResult"/> representing the result of the autocomplete interaction execution.</returns>
    public ValueTask<IExecutionResult> ExecuteAutocompleteAsync(TAutocompleteContext context, IServiceProvider? serviceProvider = null)
    {
        var data = context.Interaction.Data;
        if (TryGetApplicationCommandInfo(data.Id, out var command))
            return ((IAutocompleteInfo)command).InvokeAutocompleteAsync(context, data.Options, serviceProvider);

        return new(new NotFoundResult("Command not found."));
    }

    private protected override void InitializeAutocomplete(IAutocompleteInfo autocompleteInfo)
    {
        autocompleteInfo.InitializeAutocomplete<TAutocompleteContext>(_configuration.ServiceResolverProvider);
    }
}

/// <summary>
/// Represents a service for managing application commands.
/// </summary>
/// <typeparam name="TContext">The application command context.</typeparam>
/// <param name="configuration"><inheritdoc cref="Configuration" path="/summary" /></param>
public class ApplicationCommandService<TContext>(ApplicationCommandServiceConfiguration<TContext>? configuration = null) : IApplicationCommandService where TContext : IApplicationCommandContext
{
    private protected readonly ApplicationCommandServiceConfiguration<TContext> _configuration = configuration ?? ApplicationCommandServiceConfiguration<TContext>.Default;

    private readonly List<ApplicationCommandInfo<TContext>> _globalCommandsToCreate = [];
    private readonly Dictionary<ulong, List<ApplicationCommandInfo<TContext>>> _guildCommandsToCreate = [];

    private FrozenDictionary<ulong, ApplicationCommandInfo<TContext>> _commands = FrozenDictionary<ulong, ApplicationCommandInfo<TContext>>.Empty;

    IReadOnlyList<IApplicationCommandInfo> IApplicationCommandService.GlobalCommands => _globalCommandsToCreate;

    IEnumerable<GuildCommands> IApplicationCommandService.GuildCommands => _guildCommandsToCreate.Select(c => new GuildCommands(c.Key, c.Value));

    /// <summary>
    /// Configuration for the application command service.
    /// </summary>
    public ApplicationCommandServiceConfiguration<TContext> Configuration => _configuration;

    /// <summary>
    /// Gets the registered application commands.
    /// </summary>
    /// <returns>A read-only dictionary containing the application commands, where the key is the command ID and the value is the command information.</returns>
    public IReadOnlyDictionary<ulong, ApplicationCommandInfo<TContext>> GetCommands() => _commands;

    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly)
    {
        foreach (var type in ServiceHelpers.GetModules(typeof(BaseApplicationCommandModule<TContext>), assembly))
            AddModuleCore(type);
    }

    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseApplicationCommandModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from '{typeof(BaseApplicationCommandModule<TContext>)}'.");

        AddModuleCore(type);
    }

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] T>()
    {
        AddModule(typeof(T));
    }

    private void AddModuleCore([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type)
    {
        var configuration = _configuration;

        bool slashCommandGroup = false;

        foreach (var slashCommandAttribute in type.GetCustomAttributes<SlashCommandAttribute>())
        {
            SlashCommandGroupInfo<TContext> slashCommandGroupInfo = new(type, slashCommandAttribute, configuration);
            InitializeAutocomplete(slashCommandGroupInfo);
            AddCommandInfo(slashCommandGroupInfo);

            slashCommandGroup = true;
        }

        bool entryPointCommand = false;

        foreach (var entryPointCommandAttribute in type.GetCustomAttributes<EntryPointCommandAttribute>())
        {
            if (slashCommandGroup)
                throw new InvalidOperationException($"The type '{type}' cannot have both a slash command and an entry point command defined.");

            EntryPointCommandInfo<TContext> entryPointCommandInfo = new(entryPointCommandAttribute, configuration);
            AddCommandInfo(entryPointCommandInfo);

            entryPointCommand = true;
        }

        if (slashCommandGroup || entryPointCommand)
            return;

        foreach (var method in type.GetMethods())
        {
            foreach (var applicationCommandAttribute in method.GetCustomAttributes<ApplicationCommandAttribute>())
            {
                if (applicationCommandAttribute is SlashCommandAttribute slashCommandAttribute)
                {
                    SlashCommandInfo<TContext> slashCommandInfo = new(method, type, slashCommandAttribute, configuration);
                    InitializeAutocomplete(slashCommandInfo);
                    AddCommandInfo(slashCommandInfo);
                }

                if (applicationCommandAttribute is UserCommandAttribute userCommandAttribute)
                    AddCommandInfo(new UserCommandInfo<TContext>(method, type, userCommandAttribute, configuration));

                if (applicationCommandAttribute is MessageCommandAttribute messageCommandAttribute)
                    AddCommandInfo(new MessageCommandInfo<TContext>(method, type, messageCommandAttribute, configuration));

                if (applicationCommandAttribute is EntryPointCommandAttribute entryPointCommandAttribute)
                    AddCommandInfo(new EntryPointCommandInfo<TContext>(method, type, entryPointCommandAttribute, configuration));
            }
        }
    }

    public void AddSlashCommand(string name,
                                string description,
                                Delegate handler,
                                Permissions? defaultGuildUserPermissions = null,
                                bool? dMPermission = null,
                                bool defaultPermission = true,
                                IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                IEnumerable<InteractionContextType>? contexts = null,
                                bool nsfw = false,
                                ulong? guildId = null)
    {
        SlashCommandInfo<TContext> slashCommandInfo = new(name,
                                                          description,
                                                          handler,
                                                          defaultGuildUserPermissions,
                                                          dMPermission,
                                                          defaultPermission,
                                                          integrationTypes,
                                                          contexts,
                                                          nsfw,
                                                          guildId,
                                                          _configuration);
        InitializeAutocomplete(slashCommandInfo);
        AddCommandInfo(slashCommandInfo);
    }

    public void AddSlashCommand(string name,
                                string description,
                                Action<SlashCommandBuilder> builder,
                                Permissions? defaultGuildUserPermissions = null,
                                bool? dMPermission = null,
                                bool defaultPermission = true,
                                IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                IEnumerable<InteractionContextType>? contexts = null,
                                bool nsfw = false,
                                ulong? guildId = null)
    {
        SlashCommandGroupInfo<TContext> slashCommandGroupInfo = new(name,
                                                                    description,
                                                                    builder,
                                                                    defaultGuildUserPermissions,
                                                                    dMPermission,
                                                                    defaultPermission,
                                                                    integrationTypes,
                                                                    contexts,
                                                                    nsfw,
                                                                    guildId,
                                                                    _configuration);

        InitializeAutocomplete(slashCommandGroupInfo);
        AddCommandInfo(slashCommandGroupInfo);
    }

    public void AddUserCommand(string name,
                               Delegate handler,
                               Permissions? defaultGuildUserPermissions = null,
                               bool? dMPermission = null,
                               bool defaultPermission = true,
                               IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                               IEnumerable<InteractionContextType>? contexts = null,
                               bool nsfw = false,
                               ulong? guildId = null)
    {
        AddCommandInfo(new UserCommandInfo<TContext>(name,
                                                     handler,
                                                     defaultGuildUserPermissions,
                                                     dMPermission,
                                                     defaultPermission,
                                                     integrationTypes,
                                                     contexts,
                                                     nsfw,
                                                     guildId,
                                                     _configuration));
    }

    public void AddMessageCommand(string name,
                                  Delegate handler,
                                  Permissions? defaultGuildUserPermissions = null,
                                  bool? dMPermission = null,
                                  bool defaultPermission = true,
                                  IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                  IEnumerable<InteractionContextType>? contexts = null,
                                  bool nsfw = false,
                                  ulong? guildId = null)
    {
        AddCommandInfo(new MessageCommandInfo<TContext>(name,
                                                        handler,
                                                        defaultGuildUserPermissions,
                                                        dMPermission,
                                                        defaultPermission,
                                                        integrationTypes,
                                                        contexts,
                                                        nsfw,
                                                        guildId,
                                                        _configuration));
    }

    public void AddEntryPointCommand(string name,
                                     string description,
                                     Delegate? handler = null,
                                     Permissions? defaultGuildUserPermissions = null,
                                     bool? dMPermission = null,
                                     bool defaultPermission = true,
                                     IEnumerable<ApplicationIntegrationType>? integrationTypes = null,
                                     IEnumerable<InteractionContextType>? contexts = null,
                                     bool nsfw = false,
                                     ulong? guildId = null)
    {
        AddCommandInfo(new EntryPointCommandInfo<TContext>(name,
                                                           description,
                                                           handler,
                                                           defaultGuildUserPermissions,
                                                           dMPermission,
                                                           defaultPermission,
                                                           integrationTypes,
                                                           contexts,
                                                           nsfw,
                                                           guildId,
                                                           _configuration));
    }

    void IApplicationCommandService.SetCommands(IEnumerable<KeyValuePair<ulong, IApplicationCommandInfo>> commands)
    {
        _commands = commands.ToFrozenDictionary(c => c.Key, c => (ApplicationCommandInfo<TContext>)c.Value);
    }

    int IApplicationCommandService.GetApproximateCommandsCount(bool includeGuildCommands)
        => includeGuildCommands ? _globalCommandsToCreate.Count + _guildCommandsToCreate.Count : _globalCommandsToCreate.Count;

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

    /// <summary>
    /// Registers the application commands defined in this service to the Discord application.
    /// </summary>
    /// <param name="client">Client used to send the commands to Discord.</param>
    /// <param name="applicationId">ID of the application to which the commands will be registered.</param>
    /// <param name="includeGuildCommands">Indicates whether to include guild-specific commands in the registration.</param>
    /// <param name="properties">Request properties for the registration.</param>
    /// <param name="cancellationToken">Cancellation token used to cancel the operation.</param>
    /// <returns>The list of created application commands.</returns>
    public async Task<IReadOnlyList<ApplicationCommand>> CreateCommandsAsync(RestClient client, ulong applicationId, bool includeGuildCommands = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        var globalCommandsToCreate = _globalCommandsToCreate;
        int globalCount = globalCommandsToCreate.Count;
        var globalProperties = new ApplicationCommandProperties[globalCount];

        for (int i = 0; i < globalCount; i++)
            globalProperties[i] = await globalCommandsToCreate[i].GetRawValueAsync(cancellationToken).ConfigureAwait(false);

        var created = await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, globalProperties, properties, cancellationToken).ConfigureAwait(false);

        int count = ((IApplicationCommandService)this).GetApproximateCommandsCount(includeGuildCommands);
        List<KeyValuePair<ulong, ApplicationCommandInfo<TContext>>> commands = new(count);
        List<ApplicationCommand> result = new(count);

        foreach (var (command, commandInfo) in created.Zip(globalCommandsToCreate))
        {
            commands.Add(new(command.Id, commandInfo));
            result.Add(command);
        }

        if (includeGuildCommands)
        {
            foreach (var guildCommandsPair in _guildCommandsToCreate)
            {
                var guildCommands = guildCommandsPair.Value;
                var guildCount = guildCommands.Count;
                var guildProperties = new ApplicationCommandProperties[guildCount];

                for (int i = 0; i < guildCount; i++)
                    guildProperties[i] = await guildCommands[i].GetRawValueAsync(cancellationToken).ConfigureAwait(false);

                var guildCreated = await client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, guildCommandsPair.Key, guildProperties, properties, cancellationToken).ConfigureAwait(false);
                foreach (var (command, commandInfo) in guildCreated.Zip(guildCommands))
                {
                    commands.Add(new(command.Id, commandInfo));
                    result.Add(command);
                }
            }
        }

        _commands = commands.ToFrozenDictionary();

        return result;
    }

    /// <summary>
    /// Executes an application command.
    /// </summary>
    /// <param name="context">Context containing the data for the application command execution.</param>
    /// <param name="serviceProvider">Service provider for dependency injection.</param>
    /// <returns>An <see cref="IExecutionResult"/> representing the result of the command execution.</returns>
    public ValueTask<IExecutionResult> ExecuteAsync(TContext context, IServiceProvider? serviceProvider = null)
    {
        if (TryGetApplicationCommandInfo(context.Interaction.Data.Id, out var command))
            return command.InvokeAsync(context, _configuration, serviceProvider);

        return new(new NotFoundResult("Command not found."));
    }

    private protected bool TryGetApplicationCommandInfo(ulong commandId, [MaybeNullWhen(false)] out ApplicationCommandInfo<TContext> result)
    {
        return _commands.TryGetValue(commandId, out result);
    }

    private protected virtual void InitializeAutocomplete(IAutocompleteInfo autocompleteInfo)
    {
    }
}
