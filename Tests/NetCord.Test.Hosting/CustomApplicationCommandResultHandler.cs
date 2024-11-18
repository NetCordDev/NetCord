using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

internal class CustomApplicationCommandResultHandler : IApplicationCommandResultHandler<ApplicationCommandContext>
{
    private static readonly ApplicationCommandResultHandler<ApplicationCommandContext> _defaultHandler = new(MessageFlags.Ephemeral);

    public ValueTask HandleResultAsync(IExecutionResult result, ApplicationCommandContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        logger.LogInformation("Handling result of an application command");

        return _defaultHandler.HandleResultAsync(result, context, client, logger, services);
    }
}
