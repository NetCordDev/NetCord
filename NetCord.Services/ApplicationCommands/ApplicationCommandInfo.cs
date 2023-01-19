using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandInfo<TContext> : IApplicationCommandInfo where TContext : IApplicationCommandContext
{
    public Type DeclaringType { get; }
    public bool Static { get; }
    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public string? Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public Permissions? DefaultGuildUserPermissions { get; }
    public bool DMPermission { get; }
    public bool DefaultPermission { get; }
    public bool Nsfw { get; }
    public ulong? GuildId { get; }
    public Func<object?[], Task> InvokeAsync { get; }
    public IReadOnlyList<SlashCommandParameter<TContext>>? Parameters { get; }
    public IReadOnlyDictionary<string, IAutocompleteProvider>? Autocompletes { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }
    public ApplicationCommandType Type { get; }

    internal ApplicationCommandInfo(MethodInfo method, SlashCommandAttribute slashCommandAttribute, ApplicationCommandServiceConfiguration<TContext> configuration) : this(method, attribute: slashCommandAttribute, configuration)
    {
        Type = ApplicationCommandType.ChatInput;
        Description = slashCommandAttribute.Description;
        if (slashCommandAttribute.DescriptionTranslationsProviderType != null)
            DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(slashCommandAttribute.DescriptionTranslationsProviderType)!;

        var parameters = method.GetParameters();
        var parametersLength = parameters.Length;

        Type[] types;
        int start;
        if (Static)
        {
            types = new Type[parametersLength + 1];
            start = 0;
        }
        else
        {
            types = new Type[parametersLength + 2];
            types[0] = DeclaringType;
            start = 1;
        }
        types[^1] = typeof(Task);

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
            SlashCommandParameter<TContext> newP = new(parameter, method, configuration);
            p[i] = newP;
            var autocompleteProvider = newP.AutocompleteProvider;
            if (autocompleteProvider != null)
                autocompletes.Add(newP.Name, autocompleteProvider);

            types[start++] = parameter.ParameterType;
        }
        Parameters = p;
        Autocompletes = autocompletes;

        var invoke = method.CreateDelegate(Expression.GetDelegateType(types)).DynamicInvoke;
        InvokeAsync = Unsafe.As<Func<object?[], Task>>((object?[] p) =>
        {
            try
            {
                return invoke(p);
            }
            catch (Exception ex)
            {
                throw ex.InnerException!;
            }
        });
    }

    internal ApplicationCommandInfo(MethodInfo method, UserCommandAttribute userCommandAttribute, ApplicationCommandServiceConfiguration<TContext> configuration) : this(method, attribute: userCommandAttribute, configuration)
    {
        Type = ApplicationCommandType.User;

        if (method.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"User commands must be parameterless.", method);
        Type[] types;
        if (Static)
            types = new Type[1];
        else
        {
            types = new Type[2];
            types[0] = DeclaringType;
        }
        types[^1] = typeof(Task);

        var invoke = method.CreateDelegate(Expression.GetDelegateType(types)).DynamicInvoke;
        InvokeAsync = Unsafe.As<Func<object?[], Task>>((object?[] p) =>
        {
            try
            {
                return invoke(p);
            }
            catch (Exception ex)
            {
                throw ex.InnerException!;
            }
        });
    }

    internal ApplicationCommandInfo(MethodInfo method, MessageCommandAttribute messageCommandAttribute, ApplicationCommandServiceConfiguration<TContext> configuration) : this(method, attribute: messageCommandAttribute, configuration)
    {
        Type = ApplicationCommandType.Message;

        if (method.GetParameters().Length > 0)
            throw new InvalidDefinitionException($"Message commands must be parameterless.", method);
        Type[] types;
        if (Static)
            types = new Type[1];
        else
        {
            types = new Type[2];
            types[0] = DeclaringType;
        }
        types[^1] = typeof(Task);

        var invoke = method.CreateDelegate(Expression.GetDelegateType(types)).DynamicInvoke;
        InvokeAsync = Unsafe.As<Func<object?[], Task>>((object?[] p) =>
        {
            try
            {
                return invoke(p);
            }
            catch (Exception ex)
            {
                throw ex.InnerException!;
            }
        });
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ApplicationCommandInfo(MethodInfo method, ApplicationCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        if (method.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Application commands must return '{typeof(Task).FullName}'.", method);

        DeclaringType = method.DeclaringType!;
        Static = method.IsStatic;
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
