namespace NetCord.Commands
{
    public abstract class CommandModule<TContext> : BaseCommandModule<TContext> where TContext : CommandContext
    {
        public Task<Message> ReplyAsync(string content, bool replyMention = false)
        {
            MessageBuilder messageBuilder = new()
            {
                Content = content,
                MessageReference = new(Context.Message),
                AllowedMentions = new()
                {
                    ReplyMention = replyMention
                }
            };
            return Context.Channel.SendMessageAsync(messageBuilder.Build());
        }
        public Task<Message> SendAsync(string content) => Context.Channel.SendMessageAsync(content);
        public Task<Message> SendAsync(BuiltMessage message) => Context.Channel.SendMessageAsync(message);
    }

    public abstract class CommandModule : CommandModule<CommandContext>
    {
    }
}
