namespace NetCord.Services;

public class RequireContextAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public RequiredContext RequiredContext { get; }

    public string Format { get; }

    /// <param name="requiredContext"></param>
    /// <param name="format">{0} - required context</param>
    public RequireContextAttribute(RequiredContext requiredContext, string? format = null)
    {
        if (!Enum.IsDefined(requiredContext))
            throw new ArgumentException("Invalid value", nameof(requiredContext));
        RequiredContext = requiredContext;
        Format = format ?? "Required context: {0}";
    }

    public override Task EnsureCanExecuteAsync(TContext context)
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
        return Task.CompletedTask;
    }
}

public enum RequiredContext
{
    Guild,
    GroupDM,
    DM,
}

public class InvalidContextException : Exception
{
    public RequiredContext MissingContext { get; }

    internal InvalidContextException(string message, RequiredContext missingContext) : base(message)
    {
        MissingContext = missingContext;
    }
}