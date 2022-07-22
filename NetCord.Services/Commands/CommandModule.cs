using NetCord.Rest;

namespace NetCord.Services.Commands;

public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : CommandContext
{
    public Task<RestMessage> ReplyAsync(string content, bool replyMention = false, bool failIfNotExists = true, RequestProperties? properties = null)
    {
        MessageProperties message = new()
        {
            Content = content,
            MessageReference = new(Context.Message, failIfNotExists),
            AllowedMentions = new()
            {
                ReplyMention = replyMention
            }
        };
        return Context.Client.Rest.SendMessageAsync(Context.Message.ChannelId, message, properties);
    }

    public Task<RestMessage> SendAsync(MessageProperties message, RequestProperties? properties = null) => Context.Client.Rest.SendMessageAsync(Context.Message.ChannelId, message, properties);
}