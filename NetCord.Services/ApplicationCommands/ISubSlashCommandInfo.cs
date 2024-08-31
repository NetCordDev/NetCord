using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface ISubSlashCommandInfo<TContext> : IAutocompleteInfo where TContext : IApplicationCommandContext
{
    public ValueTask<IExecutionResult> InvokeAsync(TContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);

    public ValueTask<ApplicationCommandOptionProperties> GetRawValueAsync(CancellationToken cancellationToken = default);
}
