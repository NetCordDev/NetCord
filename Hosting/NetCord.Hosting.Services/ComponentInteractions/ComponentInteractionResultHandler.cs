using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public class ComponentInteractionResultHandler<TContext>(MessageFlags? messageFlags = null) : IComponentInteractionResultHandler<TContext> where TContext : IComponentInteractionContext
{
    public ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return default;

        var resultMessage = failResult.Message;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an interaction of custom ID '{Id}' failed with an exception", context.Interaction.Id);
        else
            logger.LogDebug("Execution of an interaction of custom ID '{Id}' failed with '{Message}'", context.Interaction.Id, resultMessage);

        var message = new InteractionMessageProperties()
        {
            Content = resultMessage,
            Flags = messageFlags,
        };

        return new(context.Interaction.SendResponseAsync(InteractionCallback.Message(message)));
    }
}
