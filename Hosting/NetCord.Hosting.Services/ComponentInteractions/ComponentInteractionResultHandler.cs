using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public class ComponentInteractionResultHandler<TContext> : IComponentInteractionResultHandler<TContext>
    where TContext : IComponentInteractionContext
{
    public static ComponentInteractionResultHandler<TContext> Default => new();

    public static ComponentInteractionResultHandler<TContext> Ephemeral => new EphemeralComponentInteractionResultHandler();

    protected ComponentInteractionResultHandler()
    {
    }

    private sealed class EphemeralComponentInteractionResultHandler : ComponentInteractionResultHandler<TContext>
    {
        public override ValueTask<InteractionMessageProperties> GetFailMessageAsync(IFailResult failResult, TContext context, IServiceProvider services)
        {
            return new(new InteractionMessageProperties
            {
                Content = failResult.Message,
                Flags = MessageFlags.Ephemeral,
            });
        }
    }

    public async ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return;

        var interaction = context.Interaction;

        var customId = interaction.Data.CustomId;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an interaction of custom ID '{Id}' failed with an exception", customId);
        else
            logger.LogDebug("Execution of an interaction of custom ID '{Id}' failed with '{Message}'", customId, failResult.Message);

        var response = await GetFailMessageAsync(failResult, context, services).ConfigureAwait(false);

        await interaction.SendResponseAsync(InteractionCallback.Message(response)).ConfigureAwait(false);
    }

    public virtual ValueTask<InteractionMessageProperties> GetFailMessageAsync(IFailResult failResult, TContext context, IServiceProvider services)
    {
        return new(new InteractionMessageProperties
        {
            Content = failResult.Message,
        });
    }
}
