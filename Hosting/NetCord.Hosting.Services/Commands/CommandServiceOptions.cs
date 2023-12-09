using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public class CommandServiceOptions<TContext> where TContext : ICommandContext
{
    public CommandServiceConfiguration<TContext> Configuration { get; set; } = CommandServiceConfiguration<TContext>.Default;

    public string? Prefix { get; set; }

    public IReadOnlyList<string>? Prefixes { get; set; }

    public Func<Message, GatewayClient, IServiceProvider, ValueTask<int>>? GetPrefixLengthAsync { get; set; }

    public Func<Message, GatewayClient, IServiceProvider, TContext>? CreateContext { get; set; }

    public Func<Exception, Message, GatewayClient, ILogger, IServiceProvider, ValueTask> HandleExceptionAsync { get; set; } = (exception, message, client, logger, services) =>
    {
        logger.LogInformation(exception, "An exception occurred while executing a command with content '{Content}'", message.Content);
        return new(message.ReplyAsync(exception.Message));
    };
}
