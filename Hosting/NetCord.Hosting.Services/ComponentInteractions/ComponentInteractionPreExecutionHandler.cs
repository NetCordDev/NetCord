using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public class ComponentInteractionPreExecutionHandler<TContext> : IComponentInteractionPreExecutionHandler<TContext>
    where TContext : IComponentInteractionContext
{
    public ValueTask<PreExecutionResult> HandleAsync(TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        logger.LogDebug("Received a component interaction with custom ID '{Id}'", context.Interaction.Data.CustomId);

        return new(PreExecutionResult.Continue);
    }
}
