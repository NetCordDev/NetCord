using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandService<TContext, TAutocompleteContext>(ApplicationCommandServiceConfiguration<TContext>? configuration = null) : ApplicationCommandService<TContext>(configuration) where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    public ValueTask<IExecutionResult> ExecuteAutocompleteAsync(TAutocompleteContext context, IServiceProvider? serviceProvider = null)
    {
        try
        {
            var data = context.Interaction.Data;
            if (_storage.TryGetCommand(data, out var command) && command is IAutocompleteInfo autocompleteInfo)
                return autocompleteInfo.InvokeAutocompleteAsync(context, data.Options, serviceProvider);
        }
        catch (Exception exception)
        {
            return new(new ExecutionExceptionResult(exception));
        }

        return new(NotFoundResult.Command);
    }

    private protected override void OnAutocompleteAdd(IAutocompleteInfo autocompleteInfo)
    {
        autocompleteInfo.InitializeAutocomplete<TAutocompleteContext>(_configuration.ServiceResolverProvider);
    }
}

public class ApplicationCommandService<TContext> : IApplicationCommandService where TContext : IApplicationCommandContext
{
    public ApplicationCommandService(ApplicationCommandServiceConfiguration<TContext>? configuration = null)
    {
        if (configuration is null)
        {
            _configuration = ApplicationCommandServiceConfiguration<TContext>.Default;
            _storage = new NameAndTypeApplicationCommandServiceStorage<TContext>();
        }
        else
        {
            _configuration = configuration;
            _storage = configuration.Storage ?? new NameAndTypeApplicationCommandServiceStorage<TContext>();
        }
    }

    private protected readonly ApplicationCommandServiceConfiguration<TContext> _configuration;
    private protected IApplicationCommandServiceStorage<TContext> _storage;

    private readonly List<ApplicationCommandInfo<TContext>> _commands = [];

    IReadOnlyList<IApplicationCommandInfo> IApplicationCommandService.Commands => _commands;

    IReadOnlyList<IApplicationCommandInfo> IApplicationCommandService.GetCommands() => [.. _commands];

    public ApplicationCommandServiceConfiguration<TContext> Configuration => _configuration;

    public IReadOnlyList<ApplicationCommandInfo<TContext>> GetCommands() => [.. _commands];

    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly)
    {
        foreach (var type in ServiceHelpers.GetTopLevelModules(typeof(BaseApplicationCommandModule<TContext>), assembly))
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
            OnAutocompleteAdd(slashCommandGroupInfo);
            AddCommandInfo(slashCommandGroupInfo);

            slashCommandGroup = true;
        }

        bool entryPointCommand = false;

        foreach (var entryPointCommandAttribute in type.GetCustomAttributes<EntryPointCommandAttribute>())
        {
            if (slashCommandGroup)
                throw new InvalidOperationException($"The type '{type}' cannot have both a slash command and an entry point command defined.");

            EntryPointCommandInfo<TContext> entryPointCommandInfo = new(type, entryPointCommandAttribute, configuration);
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
                    OnAutocompleteAdd(slashCommandInfo);
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

    public void AddSlashCommand(SlashCommandBuilder builder)
    {
        SlashCommandInfo<TContext> info = new(builder, _configuration);
        OnAutocompleteAdd(info);
        AddCommandInfo(info);
    }

    public void AddSlashCommandGroup(SlashCommandGroupBuilder builder)
    {
        SlashCommandGroupInfo<TContext> info = new(builder, _configuration);
        OnAutocompleteAdd(info);
        AddCommandInfo(info);
    }

    public void AddUserCommand(UserCommandBuilder builder)
    {
        AddCommandInfo(new UserCommandInfo<TContext>(builder, _configuration));
    }

    public void AddMessageCommand(MessageCommandBuilder builder)
    {
        AddCommandInfo(new MessageCommandInfo<TContext>(builder, _configuration));
    }

    public void AddEntryPointCommand(EntryPointCommandBuilder builder)
    {
        AddCommandInfo(new EntryPointCommandInfo<TContext>(builder, _configuration));
    }

    void IApplicationCommandService.AddRegisteredCommands(IReadOnlyList<RegisteredApplicationCommandInfo> commands)
    {
        _storage.AddRegisteredCommands([.. commands.Select(c => new RegisteredApplicationCommandInfo<TContext>(c.Command, (ApplicationCommandInfo<TContext>)c.CommandInfo))]);
    }

    private void AddCommandInfo(ApplicationCommandInfo<TContext> applicationCommandInfo)
    {
        _commands.Add(applicationCommandInfo);
        _storage.AddCommand(applicationCommandInfo);
    }

    public Task<IReadOnlyList<ApplicationCommand>> RegisterCommandsAsync(RestClient client, ulong applicationId, ulong? guildId = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        return ApplicationCommandServiceManager.RegisterCommandsAsync([this], client, applicationId, guildId, properties, cancellationToken);
    }

    public async ValueTask<IExecutionResult> ExecuteAsync(TContext context, IServiceProvider? serviceProvider = null)
    {
        try
        {
            if (_storage.TryGetCommand(context.Interaction.Data, out var command))
                return await command.InvokeAsync(context, _configuration, serviceProvider).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            return new ExecutionExceptionResult(exception);
        }

        return NotFoundResult.Command;
    }

    private protected virtual void OnAutocompleteAdd(IAutocompleteInfo autocompleteInfo)
    {
    }
}
