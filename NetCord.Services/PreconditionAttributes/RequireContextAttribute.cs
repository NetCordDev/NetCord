namespace NetCord.Services;

public class RequireContextAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public RequiredContext RequiredContext { get; }

    public RequireContextAttribute(RequiredContext requiredContext)
    {
        if (!Enum.IsDefined(requiredContext))
            throw new ArgumentException("Invalid value", nameof(requiredContext));
        RequiredContext = requiredContext;
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
            throw new InvalidContextException(RequiredContext);
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

    internal InvalidContextException(RequiredContext missingContext) : base($"Required context: {missingContext}")
    {
        MissingContext = missingContext;
    }
}