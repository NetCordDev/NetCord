using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public class CommandResultHandler<TContext>(MessageFlags? messageFlags = null) : ICommandResultHandler<TContext> where TContext : ICommandContext
{
    public ValueTask HandleResultAsync(IExecutionResult result, Message message, TContext context, GatewayClient client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return default;

        string resultMessage = failResult.Message;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of a command with content '{Content}' failed with an exception", message.Content);
        else
            logger.LogDebug("Execution of a command with content '{Content}' failed with '{Message}'", message.Content, resultMessage);

        return new(message.ReplyAsync(new()
        {
            Content = resultMessage,
            FailIfNotExists = false,
            Flags = messageFlags,
        }));
    }
}
