using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class SlashCommandGroupInfo<TContext> : ApplicationCommandInfo<TContext>, IAutocompleteInfo where TContext : IApplicationCommandContext
{
    [UnconditionalSuppressMessage("Trimming", "IL2062:Value passed to a method parameter annotated with 'DynamicallyAccessedMembersAttribute' cannot be statically determined and may not meet the attribute's requirements.", Justification = "'DynamicallyAccessedMembersAttribute' is inherited for nested types")]
    [UnconditionalSuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "'DynamicallyAccessedMembersAttribute' is inherited for nested types")]
    internal SlashCommandGroupInfo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type, SlashCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute, configuration)
    {
        Description = attribute.Description!;

        var descriptionTranslationsProviderType = attribute.DescriptionTranslationsProviderType;
        if (descriptionTranslationsProviderType is not null)
            DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(descriptionTranslationsProviderType)!;

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(type);

        Dictionary<string, ISubSlashCommandInfo<TContext>> subCommands = [];

        foreach (var method in type.GetMethods())
        {
            var subSlashCommandAttribute = method.GetCustomAttribute<SubSlashCommandAttribute>();
            if (subSlashCommandAttribute is not null)
                subCommands.Add(subSlashCommandAttribute.Name!, new SubSlashCommandInfo<TContext>(method, type, subSlashCommandAttribute, configuration));
        }

        var baseType = typeof(BaseApplicationCommandModule<TContext>);
        foreach (var nested in type.GetNestedTypes())
        {
            if (!nested.IsAssignableTo(baseType))
                continue;

            var subSlashCommandAttribute = nested.GetCustomAttribute<SubSlashCommandAttribute>();
            if (subSlashCommandAttribute is not null)
                subCommands.Add(subSlashCommandAttribute.Name!, new SubSlashCommandGroupInfo<TContext>(nested, subSlashCommandAttribute, configuration));
        }

        SubCommands = subCommands;
    }

    public string Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }
    public IReadOnlyDictionary<string, ISubSlashCommandInfo<TContext>> SubCommands { get; }

    public override async ValueTask InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);

        var slashCommandInteraction = (SlashCommandInteraction)context.Interaction;
        var option = slashCommandInteraction.Data.Options[0];
        if (!SubCommands.TryGetValue(option.Name, out var subCommand))
            throw new ApplicationCommandNotFoundException();

        await subCommand.InvokeAsync(context, option.Options!, configuration, serviceProvider).ConfigureAwait(false);
    }

    public override ApplicationCommandProperties GetRawValue()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return new SlashCommandProperties(Name, Description)
        {
            NameLocalizations = NameTranslationsProvider?.Translations,
            DefaultGuildUserPermissions = DefaultGuildUserPermissions,
            DMPermission = DMPermission,
            DefaultPermission = DefaultPermission,
            Nsfw = Nsfw,
            DescriptionLocalizations = DescriptionTranslationsProvider?.Translations,
            Options = SubCommands.Values.Select(c => c.GetRawValue()),
        };
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> InvokeAutocompleteAsync<TAutocompleteContext>(TAutocompleteContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, IServiceProvider? serviceProvider) where TAutocompleteContext : IAutocompleteInteractionContext
    {
        var option = options[0];
        if (SubCommands.TryGetValue(option.Name, out var subCommand))
            return subCommand.InvokeAutocompleteAsync(context, option.Options!, serviceProvider);

        throw new AutocompleteNotFoundException();
    }

    void IAutocompleteInfo.InitializeAutocomplete<TAutocompleteContext>()
    {
        foreach (var subCommand in SubCommands.Values)
            subCommand.InitializeAutocomplete<TAutocompleteContext>();
    }
}
