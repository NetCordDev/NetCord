using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public class CommandResultHandler<TContext> : ICommandResultHandler<TContext>
    where TContext : ICommandContext
{
    public static CommandResultHandler<TContext> Default
        => new();

    protected CommandResultHandler()
    {
    }

    public ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return default;

        var message = context.Message;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of a command with content '{Content}' failed with an exception", message.Content);
        else
            logger.LogDebug("Execution of a command with content '{Content}' failed with '{Message}'", message.Content, failResult.Message);

        var messageProperties = GetFailMessage(failResult, context, services);

        return new(client.Rest.SendMessageAsync(message.ChannelId, messageProperties));
    }

    public virtual MessageProperties GetFailMessage(IFailResult failResult, TContext context, IServiceProvider services)
    {
        return new()
        {
            MessageReference = MessageReferenceProperties.Reply(context.Message.Id, false),
            Content = failResult.Message,
        };
    }
}
