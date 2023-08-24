using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface ISubSlashCommandInfo<TContext> : IAutocompleteInfo where TContext : IApplicationCommandContext
{
    public Task InvokeAsync(TContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);

    public ApplicationCommandOptionProperties GetRawValue();
}
