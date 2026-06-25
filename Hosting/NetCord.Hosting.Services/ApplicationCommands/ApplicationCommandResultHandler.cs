using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class ApplicationCommandResultHandler<TContext>
    : IApplicationCommandResultHandler<TContext>
    where TContext : IApplicationCommandContext
{
    public static ApplicationCommandResultHandler<TContext> Default => new();

    public static ApplicationCommandResultHandler<TContext> Ephemeral => new EphemeralApplicationCommandResultHandler<TContext>();

    protected ApplicationCommandResultHandler()
    {
    }

    private class EphemeralApplicationCommandResultHandler<T> : ApplicationCommandResultHandler<T>
        where T : IApplicationCommandContext
    {
        public override InteractionMessageProperties GetFailMessage(IFailResult failResult, T context, IServiceProvider services)
        {
            var message = base.GetFailMessage(failResult, context, services);
            message.Flags = MessageFlags.Ephemeral;
            return message;
        }
    }

    public ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return default;

        var resultMessage = failResult.Message;

        var interaction = context.Interaction;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an application command of name '{Name}' failed with an exception", interaction.Data.Name);
        else
            logger.LogDebug("Execution of an application command of name '{Name}' failed with '{Message}'", interaction.Data.Name, resultMessage);

        var messageProperties = GetFailMessage(failResult, context, services);

        return new(interaction.SendResponseAsync(InteractionCallback.Message(messageProperties)));
    }

    public virtual InteractionMessageProperties GetFailMessage(IFailResult failResult, TContext context, IServiceProvider services)
    {
        return new()
        {
            Content = failResult.Message,
        };
    }
}
