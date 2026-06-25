using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public class ComponentInteractionResultHandler<TContext> 
    : IComponentInteractionResultHandler<TContext>
    where TContext : IComponentInteractionContext
{
    public static ComponentInteractionResultHandler<TContext> Ephemeral
        => new EphemeralComponentInteractionResultHandler<TContext>();

    public static ComponentInteractionResultHandler<TContext> Default
        => new();

    protected ComponentInteractionResultHandler()
    {
    }

    private class EphemeralComponentInteractionResultHandler<T> : ComponentInteractionResultHandler<T>
        where T : IComponentInteractionContext
    {
        public override InteractionMessageProperties GetFailMessage(IFailResult failResult, T context, IServiceProvider services)
        {
            var message = base.GetFailMessage(failResult, context, services);
            message.Flags = MessageFlags.Ephemeral;
            return message;
        }
    }

    public ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return default;

        var resultMessage = failResult.Message;

        var interaction = context.Interaction;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an interaction of custom ID '{Id}' failed with an exception", interaction.Data.CustomId);
        else
            logger.LogDebug("Execution of an interaction of custom ID '{Id}' failed with '{Message}'", interaction.Data.CustomId, resultMessage);

        var messageProperties = GetFailMessage(failResult, context, services);

        return new(interaction.SendResponseAsync(InteractionCallback.Message(messageProperties)));
    }

    public virtual InteractionMessageProperties GetFailMessage(IFailResult failResult, TContext context, IServiceProvider services)
    {
        return new()
        {
            Content = failResult.Message,
        };
    }
}
