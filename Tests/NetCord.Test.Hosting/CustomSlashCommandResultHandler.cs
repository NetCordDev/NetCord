using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Hosting;

internal class CustomSlashCommandResultHandler : ApplicationCommandResultHandler<SlashCommandInteraction, SlashCommandContext>
{
    public override ValueTask HandleResultAsync(IExecutionResult result, SlashCommandInteraction interaction, SlashCommandContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        logger.LogInformation("Handling result of slash command");

        return base.HandleResultAsync(result, interaction, context, client, logger, services);
    }
}
