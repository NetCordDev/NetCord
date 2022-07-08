namespace NetCord.Services;

public class RequireNsfwAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public string Message { get; }

    public RequireNsfwAttribute(string? message = null)
    {
        Message = message ?? "Required nsfw channel";
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