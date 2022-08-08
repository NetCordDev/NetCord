namespace NetCord.Services;

public class RequireNsfwAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public string Message { get; }

    public RequireNsfwAttribute(string message = "Required nsfw channel.")
    {
        Message = message;
    }

    public override Task EnsureCanExecuteAsync(TContext context)
    {
        if (context.Channel is TextGuildChannel guildChannel && !guildChannel.IsNsfw)
            throw new RequiredNsfwException(Message);
        return Task.CompletedTask;
    }
}

public class RequiredNsfwException : Exception
{
    public RequiredNsfwException(string message) : base(message)
    {
    }
}