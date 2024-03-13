using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class SubSlashCommandGroupInfo<TContext> : ISubSlashCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal SubSlashCommandGroupInfo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type, SubSlashCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration, ImmutableList<LocalizationPathSegment> path)
    {
        var name = Name = attribute.Name!;

        var localizationPath = LocalizationPath = path.Add(new SubSlashCommandGroupLocalizationPathSegment(name));

        LocalizationsProvider = configuration.LocalizationsProvider;

        Description = attribute.Description!;

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(type);

        List<KeyValuePair<string, ISubSlashCommandInfo<TContext>>> subCommands = [];

        foreach (var method in type.GetMethods())
        {
            foreach (var subSlashCommandAttribute in method.GetCustomAttributes<SubSlashCommandAttribute>())
                subCommands.Add(new(subSlashCommandAttribute.Name!, new SubSlashCommandInfo<TContext>(method, type, subSlashCommandAttribute, configuration, localizationPath)));
        }

        if (subCommands.Count == 0)
            throw new InvalidOperationException($"No sub commands found in '{type.FullName}'.");

        SubCommands = subCommands.ToFrozenDictionary();
    }

    public string Name { get; }
    public ILocalizationsProvider? LocalizationsProvider { get; }
    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }
    public string Description { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }
    public IReadOnlyDictionary<string, ISubSlashCommandInfo<TContext>> SubCommands { get; }

    public async ValueTask<IExecutionResult> InvokeAsync(TContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var preconditionResult = await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        if (preconditionResult is IFailResult)
            return preconditionResult;

        var option = options[0];
        if (!SubCommands.TryGetValue(option.Name, out var subCommand))
            return new NotFoundResult("Command not found.");

        return await subCommand.InvokeAsync(context, option.Options!, configuration, serviceProvider).ConfigureAwait(false);
    }

    public async ValueTask<ApplicationCommandOptionProperties> GetRawValueAsync()
    {
        var subCommands = SubCommands;
        var count = subCommands.Count;

        var options = new ApplicationCommandOptionProperties[count];
        int i = 0;
        foreach (var subCommand in subCommands.Values)
            options[i++] = await subCommand.GetRawValueAsync().ConfigureAwait(false);

        return new(ApplicationCommandOptionType.SubCommandGroup, Name, Description)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance)).ConfigureAwait(false),
            DescriptionLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(DescriptionLocalizationPathSegment.Instance)).ConfigureAwait(false),
            Options = options,
        };
    }

    public ValueTask<IExecutionResult> InvokeAutocompleteAsync<TAutocompleteContext>(TAutocompleteContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, IServiceProvider? serviceProvider) where TAutocompleteContext : IAutocompleteInteractionContext
    {
        var option = options[0];
        if (SubCommands.TryGetValue(option.Name, out var subCommand))
            return subCommand.InvokeAutocompleteAsync(context, option.Options!, serviceProvider);

        return new(new NotFoundResult("Command not found."));
    }

    void IAutocompleteInfo.InitializeAutocomplete<TAutocompleteContext>()
    {
        foreach (var subCommand in SubCommands.Values)
            subCommand.InitializeAutocomplete<TAutocompleteContext>();
    }
}
