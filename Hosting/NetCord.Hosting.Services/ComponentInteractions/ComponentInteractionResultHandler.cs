using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public class ComponentInteractionResultHandler<TInteraction, TContext> : IComponentInteractionResultHandler<TInteraction, TContext> where TInteraction : Interaction where TContext : IComponentInteractionContext
{
    public virtual ValueTask HandleResultAsync(IExecutionResult result, TInteraction interaction, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return default;

        var message = failResult.Message;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an interaction of custom ID '{Id}' failed with an exception", interaction.Id);
        else
            logger.LogDebug("Execution of an interaction of custom ID '{Id}' failed with '{Message}'", interaction.Id, message);

        return new(interaction.SendResponseAsync(InteractionCallback.Message(message)));
    }
}
