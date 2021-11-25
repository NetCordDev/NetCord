namespace NetCord.Commands;

public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : CommandContext
{
    public Task<Message> ReplyAsync(string content, bool replyMention = false, bool failIfNotExists = true) => Context.Message.ReplyAsync(content, replyMention, failIfNotExists);
    public Task<Message> SendAsync(string content) => Context.Channel.SendMessageAsync(content);
    public Task<Message> SendAsync(BuiltMessage message) => Context.Channel.SendMessageAsync(message);
}

public abstract class CommandModule : CommandModule<CommandContext>
{
}