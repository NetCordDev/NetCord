using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public interface IComponentInteractionPreExecutionHandler<TContext>
    where TContext : IComponentInteractionContext
{
    public ValueTask<PreExecutionResult> HandleAsync(TContext context, GatewayClient? client, ILogger logger, IServiceProvider services);
}
