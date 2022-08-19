namespace NetCord.Services;

public class RequireNsfwAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public string Message { get; }

    public RequireNsfwAttribute(string message = "Required nsfw channel.")
    {
        Message = message;
    }

    public override ValueTask EnsureCanExecuteAsync(TContext context)
    {
        if (context.Channel is TextGuildChannel guildChannel && !guildChannel.Nsfw)
            throw new RequiredNsfwException(Message);
        return default;
    }
}

public class RequiredNsfwException : Exception
{
    public RequiredNsfwException(string message) : base(message)
    {
    }
}