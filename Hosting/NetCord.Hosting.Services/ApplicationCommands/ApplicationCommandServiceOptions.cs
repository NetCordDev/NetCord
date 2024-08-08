using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class ApplicationCommandServiceOptions<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext
{
    public ApplicationCommandServiceConfiguration<TContext> Configuration { get; set; } = ApplicationCommandServiceConfiguration<TContext>.Default;

    public bool UseScopes { get; set; } = true;

    public Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? CreateContext { get; set; }

    public Func<IExecutionResult, TInteraction, TContext, GatewayClient?, ILogger, IServiceProvider, ValueTask> HandleResultAsync { get; set; } = (result, interaction, context, client, logger, services) =>
    {
        if (result is not IFailResult failResult)
            return default;

        var message = failResult.Message;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an application command of name '{Name}' failed with an exception", interaction.Data.Name);
        else
            logger.LogDebug("Execution of an application command of name '{Name}' failed with '{Message}'", interaction.Data.Name, message);

        return new(interaction.SendResponseAsync(InteractionCallback.Message(message)));
    };
}

public class ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext> : ApplicationCommandServiceOptions<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    public Func<AutocompleteInteraction, GatewayClient?, IServiceProvider, TAutocompleteContext>? CreateAutocompleteContext { get; set; }

    public Func<IExecutionResult, AutocompleteInteraction, GatewayClient?, ILogger, IServiceProvider, ValueTask> HandleAutocompleteResultAsync { get; set; } = (result, interaction, client, logger, services) =>
    {
        if (result is not IFailResult failResult)
            return default;

        var commandName = interaction.Data.Name;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an autocomplete for application command of name '{Name}' failed with an exception", commandName);
        else
            logger.LogDebug("Execution of an autocomplete for application command of name '{Name}' failed with '{Message}'", commandName, failResult.Message);

        return default;
    };
}
