namespace NetCord.Commands;

public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : CommandContext
{
    public Task<RestMessage> ReplyAsync(string content, bool replyMention = false, bool failIfNotExists = true) => Context.Message.ReplyAsync(content, replyMention, failIfNotExists);
    public Task<RestMessage> SendAsync(string content) => Context.Channel.SendMessageAsync(content);
    public Task<RestMessage> SendAsync(Message message) => Context.Channel.SendMessageAsync(message);
}

public abstract class CommandModule : CommandModule<CommandContext>
{
}