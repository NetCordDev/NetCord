using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public class ComponentInteractionResultHandler<TContext> 
    : IComponentInteractionResultHandler<TContext>
    where TContext : IComponentInteractionContext
{
    public static ComponentInteractionResultHandler<TContext> Default => new();

    public static ComponentInteractionResultHandler<TContext> Ephemeral => new EphemeralComponentInteractionResultHandler<TContext>();

    protected ComponentInteractionResultHandler()
    {
    }

    private class EphemeralComponentInteractionResultHandler<T> : ComponentInteractionResultHandler<T>
        where T : IComponentInteractionContext
    {
        public override async ValueTask<InteractionMessageProperties> GetFailMessageAsync(IFailResult failResult, T context, IServiceProvider services)
        {
            var message = await base.GetFailMessageAsync(failResult, context, services).ConfigureAwait(false);
            message.Flags = MessageFlags.Ephemeral;
            return message;
        }
    }

    public async ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return;

        var resultMessage = failResult.Message;

        var interaction = context.Interaction;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an interaction of custom ID '{Id}' failed with an exception", interaction.Data.CustomId);
        else
            logger.LogDebug("Execution of an interaction of custom ID '{Id}' failed with '{Message}'", interaction.Data.CustomId, resultMessage);

        var messageProperties = await GetFailMessageAsync(failResult, context, services).ConfigureAwait(false);

        await interaction.SendResponseAsync(InteractionCallback.Message(messageProperties)).ConfigureAwait(false);
    }

    public virtual ValueTask<InteractionMessageProperties> GetFailMessageAsync(IFailResult failResult, TContext context, IServiceProvider services)
    {
        return new(new InteractionMessageProperties
        {
            Content = failResult.Message,
        });
    }
}
