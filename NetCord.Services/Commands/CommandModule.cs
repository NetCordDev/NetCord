using NetCord.Rest;

namespace NetCord.Services.Commands;

public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : CommandContext
{
    public Task<RestMessage> ReplyAsync(string content, bool replyMention = false, bool failIfNotExists = true)
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
        return Context.Client.Rest.SendMessageAsync(Context.Message.ChannelId, message);
    }

    public Task<RestMessage> SendAsync(MessageProperties message) => Context.Client.Rest.SendMessageAsync(Context.Message.ChannelId, message);
}

public abstract class CommandModule : CommandModule<CommandContext>
{
}