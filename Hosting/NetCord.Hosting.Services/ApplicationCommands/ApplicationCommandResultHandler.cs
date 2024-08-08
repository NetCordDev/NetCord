using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class ApplicationCommandResultHandler<TContext>(MessageFlags? onFailResultResponseFlags = null) : IApplicationCommandResultHandler<TContext> where TContext : IApplicationCommandContext
{
    public MessageFlags? OnFailResultResponseFlags { get; set; } = onFailResultResponseFlags;

    public ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return default;

        var resultMessage = failResult.Message;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an application command of name '{Name}' failed with an exception", context.Interaction.Data.Name);
        else
            logger.LogDebug("Execution of an application command of name '{Name}' failed with '{Message}'", context.Interaction.Data.Name, resultMessage);

        var message = new InteractionMessageProperties()
        {
            Content = resultMessage,
            Flags = OnFailResultResponseFlags
        };

        return new(context.Interaction.SendResponseAsync(InteractionCallback.Message(message)));
    }
}
