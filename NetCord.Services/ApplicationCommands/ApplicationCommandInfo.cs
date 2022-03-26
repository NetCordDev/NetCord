using System.Collections.ObjectModel;
using System.Reflection;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandInfo<TContext> : IApplicationCommandInfo where TContext : IApplicationCommandContext
{
    public Type DeclaringType { get; }
    public string Name { get; }
    public ITranslateProvider? NameTranslateProvider { get; }
    public string? Description { get; }
    public ITranslateProvider? DescriptionTranslateProvider { get; }
    public bool DefaultPermission { get; init; }
    public DiscordId? GuildId { get; init; }
    public IEnumerable<DiscordId>? AllowedRoleIds { get; init; }
    public IEnumerable<DiscordId>? DisallowedRoleIds { get; init; }
    public IEnumerable<DiscordId>? AllowedUserIds { get; init; }
    public IEnumerable<DiscordId>? DisallowedUserIds { get; init; }
    public Func<object, object?[]?, Task> InvokeAsync { get; }
    public ReadOnlyCollection<SlashCommandParameter<TContext>>? Parameters { get; }
    public Dictionary<string, IAutocompleteProvider>? Autocompletes { get; }
    public ReadOnlyCollection<PreconditionAttribute<TContext>> Preconditions { get; }
    public ApplicationCommandType Type { get; }

    internal ApplicationCommandInfo(MethodInfo methodInfo, SlashCommandAttribute slashCommandAttribute, ApplicationCommandServiceOptions<TContext> options) : this(methodInfo, attribute: slashCommandAttribute)
    {
        Type = ApplicationCommandType.ChatInput;
        Description = slashCommandAttribute.Description;
        if (slashCommandAttribute.DescriptionTranslateProviderType != null)
            DescriptionTranslateProvider = (ITranslateProvider)Activator.CreateInstance(slashCommandAttribute.DescriptionTranslateProviderType)!;

        Autocompletes = new();

        var parameters = methodInfo.GetParameters();
        var parametersLength = parameters.Length;
        var p = new SlashCommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException($"Optional parameters must appear after all required parameters", methodInfo);
            SlashCommandParameter<TContext> newP = new(parameter, options);
            p[i] = newP;
            var autocompleteProvider = newP.AutocompleteProvider;
            if (autocompleteProvider != null)
                Autocompletes.Add(newP.Name, autocompleteProvider);
        }
        Parameters = new(p);
    }

    internal ApplicationCommandInfo(MethodInfo methodInfo, UserCommandAttribute userCommandAttribute) : this(methodInfo, attribute: userCommandAttribute)
    {
        Type = ApplicationCommandType.User;

        if (methodInfo.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"User commands must be parameterless", methodInfo);
    }

    internal ApplicationCommandInfo(MethodInfo methodInfo, MessageCommandAttribute messageCommandAttribute) : this(methodInfo, attribute: messageCommandAttribute)
    {
        Type = ApplicationCommandType.Message;

        if (methodInfo.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"Message commands must be parameterless", methodInfo);
    }

    private ApplicationCommandInfo(MethodInfo methodInfo, ApplicationCommandAttribute attribute)
    {
        DeclaringType = methodInfo.DeclaringType!;
        Name = attribute.Name;
        if (attribute.NameTranslateProviderType != null)
            NameTranslateProvider = (ITranslateProvider)Activator.CreateInstance(attribute.NameTranslateProviderType)!;
        DefaultPermission = attribute.DefaultPermission;
        if (attribute.GuildId != default)
        {
            GuildId = attribute.GuildId;
            if (attribute.AllowedRoleIds != null)
                AllowedRoleIds = attribute.AllowedRoleIds.Select(id => new DiscordId(id));
            if (attribute.DisallowedRoleIds != null)
                DisallowedRoleIds = attribute.DisallowedRoleIds.Select(id => new DiscordId(id));
            if (attribute.AllowedUserIds != null)
                AllowedUserIds = attribute.AllowedUserIds.Select(id => new DiscordId(id));
            if (attribute.DisallowedUserIds != null)
                DisallowedUserIds = attribute.DisallowedUserIds.Select(id => new DiscordId(id));
        }
        InvokeAsync = (obj, parameters) => (Task)methodInfo.Invoke(obj, BindingFlags.DoNotWrapExceptions, null, parameters, null)!;
        Preconditions = new(PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(methodInfo, DeclaringType));
    }

    public ApplicationCommandProperties GetRawValue()
    {
        return Type switch
        {
            ApplicationCommandType.ChatInput => new SlashCommandProperties(Name, Description!)
            {
                NameLocalizations = NameTranslateProvider?.Translations,
                DescriptionLocalizations = DescriptionTranslateProvider?.Translations,
                DefaultPermission = DefaultPermission,
                Options = Parameters!.Select(p => p.GetRawValue()),
            },
            ApplicationCommandType.User => new UserCommandProperties(Name)
            {
                NameLocalizations = NameTranslateProvider?.Translations,
                DefaultPermission = DefaultPermission,
            },
            ApplicationCommandType.Message => new MessageCommandProperties(Name)
            {
                NameLocalizations = NameTranslateProvider?.Translations,
                DefaultPermission = DefaultPermission,
            },
            _ => throw new InvalidOperationException(),
        };
    }

    public IEnumerable<ApplicationCommandPermissionProperties> GetRawPermissions()
    {
        if (AllowedRoleIds != null)
        {
            foreach (var r in AllowedRoleIds)
                yield return new(r, ApplicationCommandPermissionType.Role, true);
        }
        if (DisallowedRoleIds != null)
        {
            foreach (var r in DisallowedRoleIds)
                yield return new(r, ApplicationCommandPermissionType.Role, false);
        }
        if (AllowedUserIds != null)
        {
            foreach (var u in AllowedUserIds)
                yield return new(u, ApplicationCommandPermissionType.User, true);
        }
        if (DisallowedUserIds != null)
        {
            foreach (var u in DisallowedUserIds)
                yield return new(u, ApplicationCommandPermissionType.User, false);
        }
    }

    internal async Task EnsureCanExecuteAsync(TContext context)
    {
        foreach (var preconditionAttribute in Preconditions)
            await preconditionAttribute.EnsureCanExecuteAsync(context).ConfigureAwait(false);
    }
}