using System.Reflection;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandInfo<TContext> : IApplicationCommandInfo where TContext : IApplicationCommandContext
{
    public Type DeclaringType { get; }
    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public string? Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public Permission? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public bool Nsfw { get; }
    public ulong? GuildId { get; }
    public Func<object, object?[]?, Task> InvokeAsync { get; }
    public IReadOnlyList<SlashCommandParameter<TContext>>? Parameters { get; }
    public Dictionary<string, IAutocompleteProvider>? Autocompletes { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }
    public ApplicationCommandType Type { get; }

    internal ApplicationCommandInfo(MethodInfo method, SlashCommandAttribute slashCommandAttribute, ApplicationCommandServiceOptions<TContext> options) : this(method, attribute: slashCommandAttribute, options)
    {
        Type = ApplicationCommandType.ChatInput;
        Description = slashCommandAttribute.Description;
        if (slashCommandAttribute.DescriptionTranslationsProviderType != null)
            DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(slashCommandAttribute.DescriptionTranslationsProviderType)!;

        Autocompletes = new();

        var parameters = method.GetParameters();
        var parametersLength = parameters.Length;
        var p = new SlashCommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException($"Optional parameters must appear after all required parameters.", method);
            SlashCommandParameter<TContext> newP = new(parameter, method, options);
            p[i] = newP;
            var autocompleteProvider = newP.AutocompleteProvider;
            if (autocompleteProvider != null)
                Autocompletes.Add(newP.Name, autocompleteProvider);
        }
        Parameters = p;
    }

    internal ApplicationCommandInfo(MethodInfo method, UserCommandAttribute userCommandAttribute, ApplicationCommandServiceOptions<TContext> options) : this(method, attribute: userCommandAttribute, options)
    {
        Type = ApplicationCommandType.User;

        if (method.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"User commands must be parameterless.", method);
    }

    internal ApplicationCommandInfo(MethodInfo methodInfo, MessageCommandAttribute messageCommandAttribute, ApplicationCommandServiceOptions<TContext> options) : this(methodInfo, attribute: messageCommandAttribute, options)
    {
        Type = ApplicationCommandType.Message;

        if (methodInfo.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"Message commands must be parameterless.", methodInfo);
    }

    private ApplicationCommandInfo(MethodInfo method, ApplicationCommandAttribute attribute, ApplicationCommandServiceOptions<TContext> options)
    {
        DeclaringType = method.DeclaringType!;
        Name = attribute.Name;
        if (attribute.NameTranslationsProviderType != null)
            NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(attribute.NameTranslationsProviderType)!;
        DefaultGuildUserPermissions = attribute._defaultGuildUserPermissions;
        DMPermission = attribute._dMPermission.HasValue ? attribute._dMPermission.GetValueOrDefault() : options.DefaultDMPermission;
#pragma warning disable CS0618 // Type or member is obsolete
        DefaultPermission = attribute.DefaultPermission;
#pragma warning restore CS0618 // Type or member is obsolete
        Nsfw = attribute.Nsfw;
        if (attribute.GuildId != default)
            GuildId = attribute.GuildId;
        InvokeAsync = (obj, parameters) => (Task)method.Invoke(obj, BindingFlags.DoNotWrapExceptions, null, parameters, null)!;
        Preconditions = PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(DeclaringType, method);
    }

    public ApplicationCommandProperties GetRawValue()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return Type switch
        {
            ApplicationCommandType.ChatInput => new SlashCommandProperties(Name, Description!)
            {
                NameLocalizations = NameTranslationsProvider?.Translations,
                DescriptionLocalizations = DescriptionTranslationsProvider?.Translations,
                DefaultGuildUserPermissions = DefaultGuildUserPermissions,
                DMPermission = DMPermission,
                DefaultPermission = DefaultPermission,
                Nsfw = Nsfw,
                Options = Parameters!.Select(p => p.GetRawValue()),
            },
            ApplicationCommandType.User => new UserCommandProperties(Name)
            {
                NameLocalizations = NameTranslationsProvider?.Translations,
                DefaultGuildUserPermissions = DefaultGuildUserPermissions,
                DMPermission = DMPermission,
                DefaultPermission = DefaultPermission,
                Nsfw = Nsfw,
            },
            ApplicationCommandType.Message => new MessageCommandProperties(Name)
            {
                NameLocalizations = NameTranslationsProvider?.Translations,
                DefaultGuildUserPermissions = DefaultGuildUserPermissions,
                DMPermission = DMPermission,
                DefaultPermission = DefaultPermission,
                Nsfw = Nsfw,
            },
            _ => throw new InvalidOperationException(),
        };
#pragma warning restore CS0618 // Type or member is obsolete
    }

    internal async Task EnsureCanExecuteAsync(TContext context)
    {
        foreach (var preconditionAttribute in Preconditions)
            await preconditionAttribute.EnsureCanExecuteAsync(context).ConfigureAwait(false);
    }
}
