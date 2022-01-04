namespace NetCord.Commands;

public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : CommandContext
{
    public Task<RestMessage> ReplyAsync(string content, bool replyMention = false, bool failIfNotExists = true)
    {
        Message message = new()
        {
            Content = content,
            MessageReference = new(Context.Message, failIfNotExists),
            AllowedMentions = new()
            {
                ReplyMention = replyMention
            }
        };
        return Context.Client.Rest.Message.SendAsync(message, Context.Channel.Id);
    }

    public Task<RestMessage> SendAsync(string content) => Context.Client.Rest.Message.SendAsync(content, Context.Channel);
    public Task<RestMessage> SendAsync(Message message) => Context.Client.Rest.Message.SendAsync(message, Context.Channel);
}

public abstract class CommandModule : CommandModule<CommandContext>
{
}