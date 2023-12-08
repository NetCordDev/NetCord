using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class ApplicationCommandServiceOptions<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext
{
    public ApplicationCommandServiceConfiguration<TContext> Configuration { get; set; } = ApplicationCommandServiceConfiguration<TContext>.Default;

    public Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? CreateContext { get; set; }

    public Func<Exception, TInteraction, GatewayClient?, ILogger, IServiceProvider, ValueTask> HandleExceptionAsync { get; set; } = (exception, interaction, client, logger, services) =>
    {
        logger.LogInformation(exception, "An exception occurred while executing an application command of name '{Name}'", interaction.Data.Name);
        return new(interaction.SendResponseAsync(InteractionCallback.Message(exception.Message)));
    };
}

public class ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext> : ApplicationCommandServiceOptions<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    public Func<AutocompleteInteraction, GatewayClient?, IServiceProvider, TAutocompleteContext>? CreateAutocompleteContext { get; set; }

    public Func<Exception, AutocompleteInteraction, GatewayClient?, ILogger, IServiceProvider, ValueTask> HandleAutocompleteExceptionAsync { get; set; } = (exception, interaction, client, logger, services) =>
    {
        logger.LogError(exception, "An exception occurred while executing an application command autocomplete for parameter '{Parameter}' of application command of name '{Name}'", interaction.Data.Options.First(o => o.Focused), interaction.Data.Name);
        return default;
    };
}
