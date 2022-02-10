namespace NetCord.Services;

public class RequireNsfwAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public override Task EnsureCanExecuteAsync(TContext context)
    {
        if (context.Channel is TextGuildChannel guildChannel && !guildChannel.IsNsfw)
            throw new RequiredNsfwException();
        return Task.CompletedTask;
    }
}

public class RequiredNsfwException : Exception
{
    public RequiredNsfwException() : base("Required nsfw channel")
    {
    }
}