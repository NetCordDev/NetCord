using NetCord.Rest;

namespace NetCord.Services.Commands;

public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : ICommandContext, IGatewayClientContext
{
    public Task<RestMessage> ReplyAsync(ReplyMessageProperties replyMessage, RequestProperties? properties = null)
        => Context.Message.ReplyAsync(replyMessage, properties);

    public Task<RestMessage> SendAsync(MessageProperties message, RequestProperties? properties = null)
        => Context.Message.SendAsync(message, properties);
}
