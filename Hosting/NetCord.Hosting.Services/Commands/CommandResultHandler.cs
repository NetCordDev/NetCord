using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public class CommandResultHandler<TContext> : ICommandResultHandler<TContext>
    where TContext : ICommandContext
{
    public static CommandResultHandler<TContext> Default => new();

    protected CommandResultHandler()
    {
    }

    public async ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return;

        var message = context.Message;

        var messageContent = message.Content;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of a command with content '{Content}' failed with an exception", messageContent);
        else
            logger.LogDebug("Execution of a command with content '{Content}' failed with '{Message}'", messageContent, failResult.Message);

        var response = await GetFailMessage(failResult, context, services).ConfigureAwait(false);

        await message.SendAsync(response).ConfigureAwait(false);
    }

    public virtual ValueTask<MessageProperties> GetFailMessage(IFailResult failResult, TContext context, IServiceProvider services)
    {
        return new(new MessageProperties
        {
            MessageReference = MessageReferenceProperties.Reply(context.Message.Id, false),
            Content = failResult.Message,
        });
    }
}
