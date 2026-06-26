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

    private sealed class EphemeralApplicationCommandResultHandler<T> : ApplicationCommandResultHandler<T>
        where T : IApplicationCommandContext
    {
        public override ValueTask<InteractionMessageProperties> GetFailMessageAsync(IFailResult failResult, T context, IServiceProvider services)
        {
            return new(new InteractionMessageProperties
            {
                Content = failResult.Message,
                Flags = MessageFlags.Ephemeral,
            });
        }
    }

    public async ValueTask HandleResultAsync(IExecutionResult result, TContext context, GatewayClient? client, ILogger logger, IServiceProvider services)
    {
        if (result is not IFailResult failResult)
            return;

        var resultMessage = failResult.Message;

        var interaction = context.Interaction;

        if (failResult is IExceptionResult exceptionResult)
            logger.LogError(exceptionResult.Exception, "Execution of an application command of name '{Name}' failed with an exception", interaction.Data.Name);
        else
            logger.LogDebug("Execution of an application command of name '{Name}' failed with '{Message}'", interaction.Data.Name, resultMessage);

        var messageProperties = await GetFailMessageAsync(failResult, context, services).ConfigureAwait(false);

        await interaction.SendResponseAsync(InteractionCallback.Message(messageProperties)).ConfigureAwait(false);
    }

    public virtual ValueTask<InteractionMessageProperties> GetFailMessageAsync(IFailResult failResult, TContext context, IServiceProvider services)
    {
        return new(new InteractionMessageProperties
        {
            Content = failResult.Message,
        });
    }
}
