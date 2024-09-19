using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NetCord.Services;

public class RequireContextAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IChannelContext
{
    public RequiredContext RequiredContext { get; }

    public string Format => _format.Format;

    private readonly CompositeFormat _format;

    /// <param name="requiredContext"></param>
    /// <param name="format">{0} - required context</param>
    public RequireContextAttribute(RequiredContext requiredContext, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format = "Required context: {0}.")
    {
        if (requiredContext > RequiredContext.DM)
            throw new InvalidEnumArgumentException(nameof(requiredContext), (int)requiredContext, typeof(RequiredContext));

        RequiredContext = requiredContext;
        _format = CompositeFormat.Parse(format);
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
            return new(new InvalidContextResult(string.Format(null, _format, requiredContext), requiredContext));

        return new(PreconditionResult.Success);
    }
}

public enum RequiredContext : byte
{
    Guild,
    GroupDM,
    DM,
}

public class InvalidContextResult(string message, RequiredContext missingContext) : PreconditionFailResult(message)
{
    public RequiredContext MissingContext => missingContext;
}
