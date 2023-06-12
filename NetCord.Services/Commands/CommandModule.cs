using NetCord.Rest;

namespace NetCord.Services.Commands;

public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : ICommandContext, IGatewayClientContext
{
    public Task<RestMessage> ReplyAsync(string content, bool replyMention = false, bool failIfNotExists = true, RequestProperties? properties = null)
        => Context.Client.Rest.SendMessageAsync(Context.Message.ChannelId, new()
        {
            Content = content,
            MessageReference = new(Context.Message.Id, failIfNotExists),
            AllowedMentions = new()
            {
                ReplyMention = replyMention
            }
        }, properties);

    public Task<RestMessage> SendAsync(MessageProperties message, RequestProperties? properties = null)
        => Context.Client.Rest.SendMessageAsync(Context.Message.ChannelId, message, properties);
}
