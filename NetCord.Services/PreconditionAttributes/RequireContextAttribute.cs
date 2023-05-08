using System.Runtime.Serialization;

namespace NetCord.Services;

public class RequireContextAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public RequiredContext RequiredContext { get; }

    public string Format { get; }

    /// <param name="requiredContext"></param>
    /// <param name="format">{0} - required context</param>
    public RequireContextAttribute(RequiredContext requiredContext, string format = "Required context: {0}.")
    {
        if (!Enum.IsDefined(requiredContext))
            throw new System.ComponentModel.InvalidEnumArgumentException(nameof(requiredContext), (int)requiredContext, typeof(RequiredContext));

        RequiredContext = requiredContext;
        Format = format;
    }

    public override ValueTask EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        var channel = context.Channel;

        if (RequiredContext switch
        {
            RequiredContext.Guild => channel is not IGuildChannel,
            RequiredContext.GroupDM => channel is not GroupDMChannel,
            RequiredContext.DM => channel is not DMChannel,
            _ => throw new InvalidOperationException(),
        })
            throw new InvalidContextException(string.Format(Format, RequiredContext), RequiredContext);
        return default;
    }
}

public enum RequiredContext
{
    Guild,
    GroupDM,
    DM,
}

[Serializable]
public class InvalidContextException : Exception
{
    public RequiredContext MissingContext { get; }

    public InvalidContextException(string message, RequiredContext missingContext) : base(message)
    {
        MissingContext = missingContext;
    }

    protected InvalidContextException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        MissingContext = (RequiredContext)serializationInfo.GetInt32(nameof(MissingContext));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(MissingContext), (int)MissingContext);
    }
}
