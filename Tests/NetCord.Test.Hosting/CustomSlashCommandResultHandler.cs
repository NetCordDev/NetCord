using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

internal class CustomSlashCommandResultHandler : IApplicationCommandResultHandler<SlashCommandContext>
{
    private static readonly ApplicationCommandResultHandler<SlashCommandContext> _defaultHandler = new(MessageFlags.Ephemeral);

    public ValueTask HandleResultAsync(IExecutionResult result, SlashCommandContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        logger.LogInformation("Handling result of slash command");

        return _defaultHandler.HandleResultAsync(result, context, client, logger, services);
    }
}
