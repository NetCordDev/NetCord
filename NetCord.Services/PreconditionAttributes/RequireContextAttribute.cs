using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NetCord.Services;

#pragma warning disable IDE0290 // Use primary constructor

public class RequireContextAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IGuildContext
{
    public RequiredContext RequiredContext => GetRequiredContext(_guild);

    public string Format => _format.Format;

    private readonly bool _guild;
    private readonly CompositeFormat _format;

    /// <param name="requiredContext"></param>
    /// <param name="format">{0} - required context</param>
    public RequireContextAttribute(RequiredContext requiredContext, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format = "Required context: {0}.")
    {
        _guild = requiredContext switch
        {
            RequiredContext.Guild => true,
            RequiredContext.DM => false,
            _ => throw new InvalidEnumArgumentException(nameof(requiredContext), (int)requiredContext, typeof(RequiredContext)),
        };

        _format = CompositeFormat.Parse(format);
    }

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        var guild = _guild;
        var hasValue = context.GuildId.HasValue;

        if (guild switch
        {
            true => !hasValue,
            false => hasValue,
        })
        {
            var requiredContext = GetRequiredContext(guild);
            return new(new InvalidContextResult(string.Format(null, _format, requiredContext), requiredContext));
        }

        return new(PreconditionResult.Success);
    }

    private static RequiredContext GetRequiredContext(bool guild) => guild ? RequiredContext.Guild : RequiredContext.DM;
}

public enum RequiredContext : byte
{
    Guild,
    DM,
}

public class InvalidContextResult(string message, RequiredContext missingContext) : PreconditionFailResult(message)
{
    public RequiredContext MissingContext => missingContext;
}
