using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Interactions;

namespace NetCord.Hosting.Services.Interactions;

public class InteractionServiceOptions<TInteraction, TContext> where TInteraction : Interaction where TContext : IInteractionContext
{
    public InteractionServiceConfiguration<TContext> Configuration { get; set; } = InteractionServiceConfiguration<TContext>.Default;

    public Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? CreateContext { get; set; }

    public Func<IExecutionResult, TInteraction, GatewayClient?, ILogger, IServiceProvider, ValueTask> HandleResultAsync { get; set; } = (result, interaction, client, logger, services) =>
    {
        if (result is not IFailResult failResult)
            return default;

        var message = failResult.Message;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an interaction of custom ID '{Id}' failed with an exception", interaction.Id);
        else
            logger.LogDebug("Execution of an interaction of custom ID '{Id}' failed with '{Message}'", interaction.Id, message);

        return new(interaction.SendResponseAsync(InteractionCallback.Message(message)));
    };
}
