using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandService<TContext, TAutocompleteContext>(ApplicationCommandServiceConfiguration<TContext>? configuration = null) : ApplicationCommandService<TContext>(configuration) where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    public ValueTask<IExecutionResult> ExecuteAutocompleteAsync(TAutocompleteContext context, IServiceProvider? serviceProvider = null)
    {
        var interaction = context.Interaction;
        var data = interaction.Data;
        if (TryGetApplicationCommandInfo(data.Id, out var command))
            return ((IAutocompleteInfo)command).InvokeAutocompleteAsync(context, data.Options, serviceProvider);

        return new(new NotFoundResult("Command not found."));
    }

    private protected override void OnAutocompleteAdd(IAutocompleteInfo autocompleteInfo)
    {
        autocompleteInfo.InitializeAutocomplete<TAutocompleteContext>();
    }
}

public class ApplicationCommandService<TContext>(ApplicationCommandServiceConfiguration<TContext>? configuration = null) : IApplicationCommandService, IService where TContext : IApplicationCommandContext
{
    private protected readonly ApplicationCommandServiceConfiguration<TContext> _configuration = configuration ?? ApplicationCommandServiceConfiguration<TContext>.Default;
    private protected FrozenDictionary<ulong, ApplicationCommandInfo<TContext>> _commands = FrozenDictionary<ulong, ApplicationCommandInfo<TContext>>.Empty;

    internal readonly List<ApplicationCommandInfo<TContext>> _globalCommandsToCreate = [];
    internal readonly Dictionary<ulong, List<ApplicationCommandInfo<TContext>>> _guildCommandsToCreate = [];

    IReadOnlyList<IApplicationCommandInfo> IApplicationCommandService.GlobalCommands => _globalCommandsToCreate;

    IEnumerable<GuildCommands> IApplicationCommandService.GuildCommands => _guildCommandsToCreate.Select(c => new GuildCommands(c.Key, c.Value));

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
            throw new InvalidOperationException($"Modules must inherit from '{nameof(BaseApplicationCommandModule<TContext>)}'.");

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
            OnAutocompleteAdd(slashCommandGroupInfo);
            AddCommandInfo(slashCommandGroupInfo);

            slashCommandGroup = true;
        }

        if (slashCommandGroup)
            return;

        foreach (var method in type.GetMethods())
        {
            foreach (var applicationCommandAttribute in method.GetCustomAttributes<ApplicationCommandAttribute>())
            {
                if (applicationCommandAttribute is SlashCommandAttribute slashCommandAttribute)
                {
                    SlashCommandInfo<TContext> slashCommandInfo = new(method, type, slashCommandAttribute, configuration);
                    OnAutocompleteAdd(slashCommandInfo);
                    AddCommandInfo(slashCommandInfo);
                }

                if (applicationCommandAttribute is UserCommandAttribute userCommandAttribute)
                    AddCommandInfo(new UserCommandInfo<TContext>(method, type, userCommandAttribute, configuration));

                if (applicationCommandAttribute is MessageCommandAttribute messageCommandAttribute)
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

    public async Task<IReadOnlyList<ApplicationCommand>> CreateCommandsAsync(RestClient client, ulong applicationId, bool includeGuildCommands = false, RequestProperties? properties = null)
    {
        int count = ((IApplicationCommandService)this).GetApproximateCommandsCount(includeGuildCommands);
        List<KeyValuePair<ulong, ApplicationCommandInfo<TContext>>> commands = new(count);
        List<ApplicationCommand> result = new(count);

        var globalProperties = _globalCommandsToCreate.Select(c => c.GetRawValue());
        var created = await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationId, globalProperties, properties).ConfigureAwait(false);

        foreach (var (command, commandInfo) in created.Zip(_globalCommandsToCreate))
        {
            commands.Add(new(command.Key, commandInfo));
            result.Add(command.Value);
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
                    commands.Add(new(command.Key, commandInfo));
                    result.Add(command.Value);
                }
            }
        }

        _commands = commands.ToFrozenDictionary();

        return result;
    }

    public async ValueTask<IExecutionResult> ExecuteAsync(TContext context, IServiceProvider? serviceProvider = null)
    {
        if (TryGetApplicationCommandInfo(context.Interaction.Data.Id, out var command))
        {
            try
            {
                return await command.InvokeAsync(context, _configuration, serviceProvider).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                return new ExecutionExceptionResult(exception);
            }
        }

        return new NotFoundResult("Command not found.");
    }

    private protected bool TryGetApplicationCommandInfo(ulong commandId, [MaybeNullWhen(false)] out ApplicationCommandInfo<TContext> result)
    {
        return _commands.TryGetValue(commandId, out result);
    }

    private protected virtual void OnAutocompleteAdd(IAutocompleteInfo autocompleteInfo)
    {
    }
}
