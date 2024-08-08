using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public interface IComponentInteractionResultHandler<TInteraction, TContext> where TInteraction : Interaction where TContext : IComponentInteractionContext
{
    public ValueTask HandleResultAsync(IExecutionResult result, TInteraction interaction, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services);
}
