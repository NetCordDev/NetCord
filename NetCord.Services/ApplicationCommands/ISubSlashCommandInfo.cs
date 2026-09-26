using System.Collections.Immutable;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface ISubSlashCommandInfo<TContext> : IAutocompleteInfo where TContext : IApplicationCommandContext
{
    public string Name { get; }
    public ILocalizationsProvider? LocalizationsProvider { get; }
    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }
    public string Description { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    public ValueTask<IExecutionResult> InvokeAsync(TContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);

    public ValueTask<ApplicationCommandOptionProperties> GetRawValueAsync(CancellationToken cancellationToken = default);
}
