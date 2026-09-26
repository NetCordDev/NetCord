using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class ApplicationCommandPreExecutionHandler<TContext> : IApplicationCommandPreExecutionHandler<TContext>
    where TContext : IApplicationCommandContext
{
    public ValueTask<PreExecutionResult> HandleAsync(TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        logger.LogDebug("Received an application command with name '{Name}'", context.Interaction.Data.Name);

        return new(PreExecutionResult.Continue);
    }
}
