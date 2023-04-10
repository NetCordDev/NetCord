using System.Linq.Expressions;
using System.Reflection;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandInfo<TContext> : IApplicationCommandInfo where TContext : IApplicationCommandContext
{
    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public Permissions? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public bool Nsfw { get; }
    public ulong? GuildId { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }
    public ApplicationCommandType Type { get; }
    public string? Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public IReadOnlyList<SlashCommandParameter<TContext>>? Parameters { get; }
    public Func<object?[]?, TContext, Task> InvokeAsync { get; }
    public IReadOnlyDictionary<string, IAutocompleteProvider>? Autocompletes { get; }

    internal ApplicationCommandInfo(MethodInfo method, SlashCommandAttribute slashCommandAttribute, ApplicationCommandServiceConfiguration<TContext> configuration, bool supportsAutocomplete, Type? autocompleteBase) : this(method, attribute: slashCommandAttribute, configuration, out var declaringType)
    {
        Type = ApplicationCommandType.ChatInput;
        Description = slashCommandAttribute.Description;
        if (slashCommandAttribute.DescriptionTranslationsProviderType != null)
            DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(slashCommandAttribute.DescriptionTranslationsProviderType)!;

        var parameters = method.GetParameters();
        var parametersLength = parameters.Length;

        var p = new SlashCommandParameter<TContext>[parametersLength];
        Dictionary<string, IAutocompleteProvider> autocompletes = new();
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException($"Optional parameters must appear after all required parameters.", method);

            SlashCommandParameter<TContext> newP = new(parameter, method, configuration, supportsAutocomplete, autocompleteBase);
            p[i] = newP;
            var autocompleteProvider = newP.AutocompleteProvider;
            if (autocompleteProvider != null)
                autocompletes.Add(newP.Name, autocompleteProvider);
        }

        Parameters = p;
        InvokeAsync = CreateDelegate(method, declaringType, p);
        Autocompletes = autocompletes;
    }

    internal ApplicationCommandInfo(MethodInfo method, UserCommandAttribute userCommandAttribute, ApplicationCommandServiceConfiguration<TContext> configuration) : this(method, attribute: userCommandAttribute, configuration, out var declaringType)
    {
        Type = ApplicationCommandType.User;

        if (method.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"User commands must be parameterless.", method);

        InvokeAsync = CreateDelegate(method, declaringType, Array.Empty<SlashCommandParameter<TContext>>());
    }

    internal ApplicationCommandInfo(MethodInfo method, MessageCommandAttribute messageCommandAttribute, ApplicationCommandServiceConfiguration<TContext> configuration) : this(method, attribute: messageCommandAttribute, configuration, out var declaringType)
    {
        Type = ApplicationCommandType.Message;

        if (method.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"Message commands must be parameterless.", method);

        InvokeAsync = CreateDelegate(method, declaringType, Array.Empty<SlashCommandParameter<TContext>>());
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ApplicationCommandInfo(MethodInfo method, ApplicationCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration, out Type declaringType)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        if (method.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Application commands must return '{typeof(Task)}'.", method);

        Name = attribute.Name;
        if (attribute.NameTranslationsProviderType != null)
            NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(attribute.NameTranslationsProviderType)!;
        DefaultGuildUserPermissions = attribute._defaultGuildUserPermissions;
        DMPermission = attribute._dMPermission.HasValue ? attribute._dMPermission.GetValueOrDefault() : configuration.DefaultDMPermission;
#pragma warning disable CS0618 // Type or member is obsolete
        DefaultPermission = attribute.DefaultPermission;
#pragma warning restore CS0618 // Type or member is obsolete
        Nsfw = attribute.Nsfw;
        if (attribute._guildId.HasValue)
            GuildId = attribute._guildId.GetValueOrDefault();
        Preconditions = PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(declaringType = method.DeclaringType!, method);
    }

    private static Func<object?[]?, TContext, Task> CreateDelegate(MethodInfo method, Type declaringType, SlashCommandParameter<TContext>[] commandParameters)
    {
        var parameters = Expression.Parameter(typeof(object?[]));
        Type contextType = typeof(TContext);
        var context = Expression.Parameter(contextType);
        Expression? instance;
        if (method.IsStatic)
            instance = null;
        else
        {
            var module = Expression.Variable(declaringType);
            instance = Expression.Block(new[] { module },
                                        Expression.Assign(module, Expression.New(declaringType)),
                                        Expression.Assign(Expression.Property(module, declaringType.GetProperty(nameof(BaseApplicationCommandModule<TContext>.Context), contextType)!), context),
                                        module);
        }
        var call = Expression.Call(instance,
                                   method,
                                   commandParameters.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i)), p.Type)));
        var lambda = Expression.Lambda(call, parameters, context);
        return (Func<object?[]?, TContext, Task>)lambda.Compile();
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
        var count = Preconditions.Count;
        for (var i = 0; i < count; i++)
        {
            var preconditionAttribute = Preconditions[i];
            await preconditionAttribute.EnsureCanExecuteAsync(context).ConfigureAwait(false);
        }
    }
}
