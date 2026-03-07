using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public class CommandPreExecutionHandler<TContext> : ICommandPreExecutionHandler<TContext>
    where TContext : ICommandContext
{
    public ValueTask<PreExecutionResult> HandleAsync(TContext context, GatewayClient client, ILogger logger, IServiceProvider services)
    {
        logger.LogDebug("Received a command with content '{Content}'", context.Message.Content);

        return new(PreExecutionResult.Continue);
    }
}
