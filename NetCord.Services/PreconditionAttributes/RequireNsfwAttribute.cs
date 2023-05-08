using System.Runtime.Serialization;

namespace NetCord.Services;

public class RequireNsfwAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public string Message { get; }

    public RequireNsfwAttribute(string message = "Required nsfw channel.")
    {
        Message = message;
    }

    public override ValueTask EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        if (context.Channel is TextGuildChannel guildChannel && !guildChannel.Nsfw)
            throw new RequiredNsfwException(Message);
        return default;
    }
}

[Serializable]
public class RequiredNsfwException : Exception
{
    public RequiredNsfwException(string message) : base(message)
    {
    }

    protected RequiredNsfwException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
