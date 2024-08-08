using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class ApplicationCommandResultHandler<TInteraction, TContext> : IApplicationCommandResultHandler<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext
{
    public virtual ValueTask HandleResultAsync(IExecutionResult result, TInteraction interaction, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return default;

        var message = failResult.Message;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an application command of name '{Name}' failed with an exception", interaction.Data.Name);
        else
            logger.LogDebug("Execution of an application command of name '{Name}' failed with '{Message}'", interaction.Data.Name, message);

        return new(interaction.SendResponseAsync(InteractionCallback.Message(message)));
    }
}
