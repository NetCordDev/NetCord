namespace NetCord.Services;

public class RequireNsfwAttribute<TContext>(string message = "Required nsfw channel.") : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    private readonly RequiredNsfwResult _failResult = new(message);

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        if (context.Channel is TextGuildChannel guildChannel && !guildChannel.Nsfw)
            return new(_failResult);

        return new(PreconditionResult.Success);
    }
}

public class RequiredNsfwResult(string message) : PreconditionFailResult(message)
{
}
