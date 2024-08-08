using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public interface IApplicationCommandResultHandler<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext
{
    public ValueTask HandleResultAsync(IExecutionResult result, TInteraction interaction, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services);
}
