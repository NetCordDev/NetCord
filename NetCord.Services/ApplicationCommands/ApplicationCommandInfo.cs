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
    public Snowflake? GuildId { get; }
    public Func<object, object?[]?, Task> InvokeAsync { get; }
    public IReadOnlyList<SlashCommandParameter<TContext>>? Parameters { get; }
    public Dictionary<string, IAutocompleteProvider>? Autocompletes { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }
    public ApplicationCommandType Type { get; }

    internal ApplicationCommandInfo(MethodInfo methodInfo, SlashCommandAttribute slashCommandAttribute, ApplicationCommandServiceOptions<TContext> options) : this(methodInfo, attribute: slashCommandAttribute)
    {
        Type = ApplicationCommandType.ChatInput;
        Description = slashCommandAttribute.Description;
        if (slashCommandAttribute.DescriptionTranslationsProviderType != null)
            DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(slashCommandAttribute.DescriptionTranslationsProviderType)!;

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
                throw new InvalidDefinitionException($"Optional parameters must appear after all required parameters.", methodInfo);
            SlashCommandParameter<TContext> newP = new(parameter, options);
            p[i] = newP;
            var autocompleteProvider = newP.AutocompleteProvider;
            if (autocompleteProvider != null)
                Autocompletes.Add(newP.Name, autocompleteProvider);
        }
        Parameters = p;
    }

    internal ApplicationCommandInfo(MethodInfo methodInfo, UserCommandAttribute userCommandAttribute) : this(methodInfo, attribute: userCommandAttribute)
    {
        Type = ApplicationCommandType.User;

        if (methodInfo.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"User commands must be parameterless.", methodInfo);
    }

    internal ApplicationCommandInfo(MethodInfo methodInfo, MessageCommandAttribute messageCommandAttribute) : this(methodInfo, attribute: messageCommandAttribute)
    {
        Type = ApplicationCommandType.Message;

        if (methodInfo.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"Message commands must be parameterless.", methodInfo);
    }

    private ApplicationCommandInfo(MethodInfo methodInfo, ApplicationCommandAttribute attribute)
    {
        DeclaringType = methodInfo.DeclaringType!;
        Name = attribute.Name;
        if (attribute.NameTranslationsProviderType != null)
            NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(attribute.NameTranslationsProviderType)!;
        DefaultGuildUserPermissions = attribute.DefaultGuildUserPermissions == (Permission)((ulong)1 << 63) ? null : attribute.DefaultGuildUserPermissions;
        DMPermission = attribute.DMPermission;
#pragma warning disable CS0618 // Type or member is obsolete
        DefaultPermission = attribute.DefaultPermission;
#pragma warning restore CS0618 // Type or member is obsolete
        if (attribute.GuildId != default)
            GuildId = attribute.GuildId;
        InvokeAsync = (obj, parameters) => (Task)methodInfo.Invoke(obj, BindingFlags.DoNotWrapExceptions, null, parameters, null)!;
        Preconditions = PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(methodInfo, DeclaringType);
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
                Options = Parameters!.Select(p => p.GetRawValue()),
            },
            ApplicationCommandType.User => new UserCommandProperties(Name)
            {
                NameLocalizations = NameTranslationsProvider?.Translations,
                DefaultGuildUserPermissions = DefaultGuildUserPermissions,
                DMPermission = DMPermission,
                DefaultPermission = DefaultPermission,
            },
            ApplicationCommandType.Message => new MessageCommandProperties(Name)
            {
                NameLocalizations = NameTranslationsProvider?.Translations,
                DefaultGuildUserPermissions = DefaultGuildUserPermissions,
                DMPermission = DMPermission,
                DefaultPermission = DefaultPermission,
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