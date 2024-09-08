using NetCord.Rest;

namespace NetCord.Services.Commands;

public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : ICommandContext, IGatewayClientContext
{
    public Task<RestMessage> ReplyAsync(ReplyMessageProperties replyMessage, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => Context.Message.ReplyAsync(replyMessage, properties, cancellationToken);

    public Task<RestMessage> SendAsync(MessageProperties message, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => Context.Message.SendAsync(message, properties, cancellationToken);
}
