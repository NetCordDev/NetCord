using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services;

public class RequireNsfwAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    private readonly RequiredNsfwResult _failResult;

    public RequireNsfwAttribute([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string message = "Required nsfw channel.")
    {
        _failResult = new(message);
    }

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        if (context.Channel is TextGuildChannel guildChannel && !guildChannel.Nsfw)
            return new(_failResult);

        return new(PreconditionResult.Success);
    }
}

public class RequiredNsfwResult : PreconditionFailResult
{
    public RequiredNsfwResult(string message) : base(message)
    {
    }
}
