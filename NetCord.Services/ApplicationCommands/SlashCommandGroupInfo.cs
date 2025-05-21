using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class SlashCommandGroupInfo<TContext> : ApplicationCommandInfo<TContext>, IAutocompleteInfo where TContext : IApplicationCommandContext
{
    [UnconditionalSuppressMessage("Trimming", "IL2062:Value passed to a method parameter annotated with 'DynamicallyAccessedMembersAttribute' cannot be statically determined and may not meet the attribute's requirements.", Justification = "'DynamicallyAccessedMembersAttribute' is inherited for nested types")]
    [UnconditionalSuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "'DynamicallyAccessedMembersAttribute' is inherited for nested types")]
    internal SlashCommandGroupInfo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type,
                                   SlashCommandAttribute attribute,
                                   ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute.Name,
                                                                                                          attribute._defaultGuildUserPermissions,
                                                                                                          attribute._dMPermission,
#pragma warning disable CS0618 // Type or member is obsolete
                                                                                                          attribute.DefaultPermission,
#pragma warning restore CS0618 // Type or member is obsolete
                                                                                                          attribute.IntegrationTypes,
                                                                                                          attribute.Contexts,
                                                                                                          attribute.Nsfw,
                                                                                                          attribute._guildId,
                                                                                                          configuration)
    {
        Description = attribute.Description;

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(type);

        List<KeyValuePair<string, ISubSlashCommandInfo<TContext>>> subCommands = [];

        foreach (var method in type.GetMethods())
        {
            foreach (var subSlashCommandAttribute in method.GetCustomAttributes<SubSlashCommandAttribute>())
                subCommands.Add(new(subSlashCommandAttribute.Name!, new SubSlashCommandInfo<TContext>(method, type, subSlashCommandAttribute, configuration, LocalizationPath)));
        }

        var baseType = typeof(BaseApplicationCommandModule<TContext>);
        foreach (var nested in type.GetNestedTypes())
        {
            if (!nested.IsAssignableTo(baseType))
                continue;

            foreach (var subSlashCommandAttribute in nested.GetCustomAttributes<SubSlashCommandAttribute>())
                subCommands.Add(new(subSlashCommandAttribute.Name!, new SubSlashCommandGroupInfo<TContext>(nested, subSlashCommandAttribute, configuration, LocalizationPath)));
        }

        if (subCommands.Count == 0)
            throw new InvalidOperationException($"No sub commands found in '{type.FullName}'.");

        SubCommands = subCommands.ToFrozenDictionary();
    }

    internal SlashCommandGroupInfo(string name,
                                   string description,
                                   Action<SlashCommandBuilder> builder,
                                   Permissions? defaultGuildUserPermissions,
                                   bool? dMPermission,
                                   bool defaultPermission,
                                   IEnumerable<ApplicationIntegrationType>? integrationTypes,
                                   IEnumerable<InteractionContextType>? contexts,
                                   bool nsfw,
                                   ulong? guildId,
                                   ApplicationCommandServiceConfiguration<TContext> configuration) : base(name,
                                                                                                          defaultGuildUserPermissions,
                                                                                                          dMPermission,
                                                                                                          defaultPermission,
                                                                                                          integrationTypes,
                                                                                                          contexts,
                                                                                                          nsfw,
                                                                                                          guildId,
                                                                                                          configuration)
    {
        Description = description;

        Preconditions = [];
        
        List<KeyValuePair<string, ISubSlashCommandInfo<TContext>>> subCommands = [];

        SlashCommandBuilder slashCommandBuilder = new();
        builder(slashCommandBuilder);

        var subCommandsInfo = slashCommandBuilder.SubCommands;
        int subCommandsCount = subCommandsInfo.Count;

        for (int i = 0; i < subCommandsCount; i++)
        {
            var subCommandInfo = subCommandsInfo[i];
            SubSlashCommandInfo<TContext> subCommand = new(subCommandInfo.Name, subCommandInfo.Description, subCommandInfo.Handler, configuration, LocalizationPath);
            subCommands.Add(new(subCommandInfo.Name, subCommand));
        }

        var subCommandGroupsInfo = slashCommandBuilder.SubCommandGroups;
        int subCommandGroupsCount = subCommandGroupsInfo.Count;

        for (int i = 0; i < subCommandGroupsCount; i++)
        {
            var subCommandGroupInfo = subCommandGroupsInfo[i];
            SubSlashCommandGroupInfo<TContext> subCommandGroup = new(subCommandGroupInfo.Name, subCommandGroupInfo.Description, subCommandGroupInfo.Builder, configuration, LocalizationPath);
            subCommands.Add(new(subCommandGroupInfo.Name, subCommandGroup));
        }

        SubCommands = subCommands.ToFrozenDictionary();
    }

    public string Description { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }
    public IReadOnlyDictionary<string, ISubSlashCommandInfo<TContext>> SubCommands { get; }

    public override async ValueTask<IExecutionResult> InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var preconditionResult = await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        if (preconditionResult is IFailResult)
            return preconditionResult;

        var slashCommandInteraction = (SlashCommandInteraction)context.Interaction;
        var option = slashCommandInteraction.Data.Options[0];
        if (!SubCommands.TryGetValue(option.Name, out var subCommand))
            return new NotFoundResult("Command not found.");

        return await subCommand.InvokeAsync(context, option.Options!, configuration, serviceProvider).ConfigureAwait(false);
    }

    public override async ValueTask<ApplicationCommandProperties> GetRawValueAsync(CancellationToken cancellationToken = default)
    {
        var subCommands = SubCommands;

        var options = new ApplicationCommandOptionProperties[subCommands.Count];
        int i = 0;
        foreach (var subCommand in subCommands.Values)
            options[i++] = await subCommand.GetRawValueAsync(cancellationToken).ConfigureAwait(false);

#pragma warning disable CS0618 // Type or member is obsolete
        return new SlashCommandProperties(Name, Description)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            DefaultGuildUserPermissions = DefaultGuildUserPermissions,
            DMPermission = DMPermission,
            DefaultPermission = DefaultPermission,
            IntegrationTypes = IntegrationTypes,
            Contexts = Contexts,
            Nsfw = Nsfw,
            DescriptionLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(DescriptionLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            Options = options,
        };
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public ValueTask<IExecutionResult> InvokeAutocompleteAsync<TAutocompleteContext>(TAutocompleteContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, IServiceProvider? serviceProvider) where TAutocompleteContext : IAutocompleteInteractionContext
    {
        var option = options[0];
        if (SubCommands.TryGetValue(option.Name, out var subCommand))
            return subCommand.InvokeAutocompleteAsync(context, option.Options!, serviceProvider);

        return new(new NotFoundResult("Command not found."));
    }

    void IAutocompleteInfo.InitializeAutocomplete<TAutocompleteContext>(IServiceResolverProvider serviceResolverProvider)
    {
        foreach (var subCommand in SubCommands.Values)
            subCommand.InitializeAutocomplete<TAutocompleteContext>(serviceResolverProvider);
    }
}
