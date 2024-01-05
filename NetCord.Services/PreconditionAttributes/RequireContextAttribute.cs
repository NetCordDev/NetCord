using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services;

public class RequireContextAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public RequiredContext RequiredContext { get; }

    public string Format { get; }

    /// <param name="requiredContext"></param>
    /// <param name="format">{0} - required context</param>
    public RequireContextAttribute(RequiredContext requiredContext, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format = "Required context: {0}.")
    {
        if (!Enum.IsDefined(requiredContext))
            throw new System.ComponentModel.InvalidEnumArgumentException(nameof(requiredContext), (int)requiredContext, typeof(RequiredContext));

        RequiredContext = requiredContext;
        Format = format;
    }

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        var channel = context.Channel;

        var requiredContext = RequiredContext;

        if (requiredContext switch
        {
            RequiredContext.Guild => channel is not IGuildChannel,
            RequiredContext.GroupDM => channel is not GroupDMChannel,
            RequiredContext.DM => channel is not DMChannel,
            _ => throw new InvalidOperationException(),
        })
            return new(new InvalidContextResult(string.Format(Format, requiredContext), requiredContext));

        return new(PreconditionResult.Success);
    }
}

public enum RequiredContext
{
    Guild,
    GroupDM,
    DM,
}

public class InvalidContextResult : PreconditionFailResult
{
    public RequiredContext MissingContext { get; }

    public InvalidContextResult(string message, RequiredContext missingContext) : base(message)
    {
        MissingContext = missingContext;
    }
}
