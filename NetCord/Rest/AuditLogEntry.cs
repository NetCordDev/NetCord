using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

using NetCord.JsonModels;

namespace NetCord.Rest;

public class AuditLogEntry : ClientEntity, IJsonModel<JsonAuditLogEntry>
{
    JsonAuditLogEntry IJsonModel<JsonAuditLogEntry>.JsonModel => _jsonModel;
    private readonly JsonAuditLogEntry _jsonModel;
    private readonly JsonAuditLog _data;

    public AuditLogEntry(JsonAuditLogEntry jsonModel, JsonAuditLog data, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        _data = data;
        Changes = _jsonModel.Changes.SelectOrEmpty(c => new AuditLogChange(c));
        if (_jsonModel.Options != null)
            Options = new(_jsonModel.Options);
        ApplicationCommands = _data.ApplicationCommands.Select(c => new ApplicationCommand(c, client));
        AutoModerationRules = _data.AutoModerationRules.Select(r => new AutoModerationRule(r, client));
        GuildScheduledEvents = _data.GuildScheduledEvents.Select(e => new GuildScheduledEvent(e, _client));
        Integrations = _data.Integrations.Select(e => new Integration(e, _client));
        Threads = _data.Threads.Select(t => (GuildThread)Channel.CreateFromJson(t, _client));
        Users = _data.Users.Select(u => new User(u, _client));
        Webhooks = _data.Webhooks.Select(w => Webhook.CreateFromJson(w, client));
    }

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// Id of the affected entity.
    /// </summary>
    public ulong? TargetId => _jsonModel.TargetId;

    /// <summary>
    /// Changes made to the <see cref="TargetId"/>.
    /// </summary>
    public IEnumerable<AuditLogChange> Changes { get; }

    /// <summary>
    /// Finds specified change based on <paramref name="expression"/>.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="expression">Expression finding the change, for example: <c>channel => channel.Name</c>.</param>
    /// <param name="jsonTypeInfo"><see cref="JsonTypeInfo{TValue}"/> of the object returned by <paramref name="expression"/>.</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public AuditLogChange<TValue> GetChange<TObject, TValue>(Expression<Func<TObject, TValue>> expression, JsonTypeInfo<TValue> jsonTypeInfo) where TObject : JsonEntity
    {
        if (_jsonModel.Changes == null)
            throw new EntityNotFoundException();

        var member = GetMemberAccess(expression);
        var name = member.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>()!.Name;

        var result = _jsonModel.Changes.FirstOrDefault(c => c.Key == name);
        if (result == null)
            throw new EntityNotFoundException();

        return new(result, jsonTypeInfo);
    }

    /// <summary>
    /// User that made the changes.
    /// </summary>
    public User? User
    {
        get
        {
            if (_jsonModel.UserId == null)
                return null;
            else
            {
                var id = _jsonModel.UserId.GetValueOrDefault();
                return new(_data.Users.First(u => u.Id == id), _client);
            }
        }
    }

    /// <summary>
    /// Type of action that occurred.
    /// </summary>
    public AuditLogEvent ActionType => _jsonModel.ActionType.GetValueOrDefault();

    /// <summary>
    /// Additional info for certain event types.
    /// </summary>
    public AuditLogEntryInfo? Options { get; }

    /// <summary>
    /// Reason for the change (1-512 characters).
    /// </summary>
    public string? Reason => _jsonModel.Reason;

    /// <summary>
    /// List of application commands referenced in the audit log.
    /// </summary>
    public IEnumerable<ApplicationCommand> ApplicationCommands { get; }

    /// <summary>
    /// List of auto moderation rules referenced in the audit log.
    /// </summary>
    public IEnumerable<AutoModerationRule> AutoModerationRules { get; }

    /// <summary>
    /// List of guild scheduled events referenced in the audit log.
    /// </summary>
    public IEnumerable<GuildScheduledEvent> GuildScheduledEvents { get; }

    /// <summary>
    /// List of integration objects.
    /// </summary>
    public IEnumerable<Integration> Integrations { get; }

    /// <summary>
    /// List of threads referenced in the audit log
    /// </summary>
    public IEnumerable<GuildThread> Threads { get; }

    /// <summary>
    /// List of users referenced in the audit log.
    /// </summary>
    public IEnumerable<User> Users { get; }

    /// <summary>
    /// List of webhooks referenced in the audit log.
    /// </summary>
    public IEnumerable<Webhook> Webhooks { get; }

    #region From https://github.com/dotnet/efcore/blob/27a83b9ad5f6ce7e13c6fbdec8f50a4aa63fb811/src/EFCore/Infrastructure/ExpressionExtensions.cs
    private static MemberInfo GetMemberAccess(LambdaExpression memberAccessExpression)
        => GetInternalMemberAccess<MemberInfo>(memberAccessExpression);

    private static TMemberInfo GetInternalMemberAccess<TMemberInfo>(LambdaExpression memberAccessExpression)
        where TMemberInfo : MemberInfo
    {
        if (memberAccessExpression.Parameters.Count != 1)
            throw new ArgumentException($"Parameters.Count is {memberAccessExpression.Parameters.Count}");

        var parameterExpression = memberAccessExpression.Parameters[0];
        var memberInfo = MatchSimpleMemberAccess<TMemberInfo>(parameterExpression, memberAccessExpression.Body);

        if (memberInfo == null)
            throw new ArgumentException($"The expression '{nameof(memberAccessExpression)}' is not a valid member access expression. The expression should represent a simple property or field access: 't => t.MyProperty'.", nameof(memberAccessExpression));

        return memberInfo;
    }

    private static TMemberInfo? MatchSimpleMemberAccess<TMemberInfo>(
    Expression parameterExpression,
    Expression memberAccessExpression)
    where TMemberInfo : MemberInfo
    {
        var memberInfos = MatchMemberAccess<TMemberInfo>(parameterExpression, memberAccessExpression);

        return memberInfos?.Count == 1 ? memberInfos[0] : null;
    }

    private static IReadOnlyList<TMemberInfo>? MatchMemberAccess<TMemberInfo>(
        Expression parameterExpression,
        Expression memberAccessExpression)
        where TMemberInfo : MemberInfo
    {
        var memberInfos = new List<TMemberInfo>();

        var unwrappedExpression = RemoveTypeAs(RemoveConvert(memberAccessExpression));
        do
        {
            var memberExpression = unwrappedExpression as MemberExpression;

            if (memberExpression?.Member is not TMemberInfo memberInfo)
                return null;

            memberInfos.Insert(0, memberInfo);

            unwrappedExpression = RemoveTypeAs(RemoveConvert(memberExpression.Expression));
        }
        while (unwrappedExpression != parameterExpression);

        return memberInfos;
    }

    private static Expression? RemoveTypeAs(Expression? expression)
    {
        while (expression?.NodeType == ExpressionType.TypeAs)
            expression = ((UnaryExpression)RemoveConvert(expression)!).Operand;

        return expression;
    }

    private static Expression? RemoveConvert(Expression? expression)
    {
        if (expression is UnaryExpression unaryExpression
            && (expression.NodeType == ExpressionType.Convert
                || expression.NodeType == ExpressionType.ConvertChecked))
            return RemoveConvert(unaryExpression.Operand);

        return expression;
    }
    #endregion
}
