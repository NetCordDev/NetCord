using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public class CommandServiceOptions<TContext> where TContext : ICommandContext
{
    public CommandServiceConfiguration<TContext> Configuration { get; set; } = CommandServiceConfiguration<TContext>.Default;

    public string? Prefix { get; set; }

    public IReadOnlyList<string>? Prefixes { get; set; }

    public Func<Message, GatewayClient, IServiceProvider, ValueTask<int>>? GetPrefixLengthAsync { get; set; }

    public Func<Message, GatewayClient, IServiceProvider, TContext>? CreateContext { get; set; }

    public Func<IExecutionResult, Message, GatewayClient, ILogger, IServiceProvider, ValueTask> HandleResultAsync { get; set; } = (result, message, client, logger, services) =>
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
        }));
    };
}
