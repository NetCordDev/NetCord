using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class SubSlashCommandGroupInfo<TContext> : ISubSlashCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal SubSlashCommandGroupInfo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type, SubSlashCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        Name = attribute.Name!;

        var nameTranslationsProviderType = attribute.NameTranslationsProviderType;
        if (nameTranslationsProviderType is not null)
            NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(nameTranslationsProviderType)!;

        Description = attribute.Description!;

        var descriptionTranslationsProviderType = attribute.DescriptionTranslationsProviderType;
        if (descriptionTranslationsProviderType is not null)
            DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(descriptionTranslationsProviderType)!;

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(type);

        Dictionary<string, ISubSlashCommandInfo<TContext>> subCommands = [];

        foreach (var method in type.GetMethods())
        {
            foreach (var subSlashCommandAttribute in method.GetCustomAttributes<SubSlashCommandAttribute>())
                subCommands.Add(subSlashCommandAttribute.Name!, new SubSlashCommandInfo<TContext>(method, type, subSlashCommandAttribute, configuration));
        }

        if (subCommands.Count == 0)
            throw new InvalidOperationException($"No sub commands found in '{type.FullName}'.");

        SubCommands = subCommands;
    }

    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public string Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }
    public IReadOnlyDictionary<string, ISubSlashCommandInfo<TContext>> SubCommands { get; }

    public async ValueTask InvokeAsync(TContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);

        var option = options[0];
        if (!SubCommands.TryGetValue(option.Name, out var subCommand))
            throw new ApplicationCommandNotFoundException();

        await subCommand.InvokeAsync(context, option.Options!, configuration, serviceProvider).ConfigureAwait(false);
    }

    public ApplicationCommandOptionProperties GetRawValue()
    {
        return new(ApplicationCommandOptionType.SubCommandGroup, Name, Description)
        {
            NameLocalizations = NameTranslationsProvider?.Translations,
            DescriptionLocalizations = DescriptionTranslationsProvider?.Translations,
            Options = SubCommands.Values.Select(c => c.GetRawValue()),
        };
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
