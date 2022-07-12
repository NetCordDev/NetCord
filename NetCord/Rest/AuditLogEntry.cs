using System.Linq.Expressions;
using System.Reflection;

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
        GuildScheduledEvents = _data.GuildScheduledEvents.Select(e => new GuildScheduledEvent(e, _client));
        Integrations = _data.Integrations.Select(e => new Integration(e, _client));
        Threads = _data.Threads.Select(t => (GuildThread)Channel.CreateFromJson(t, _client));
        Users = _data.Users.Select(u => new User(u, _client));
        Webhooks = _data.Webhooks.Select(w => Webhook.CreateFromJson(w, client));
    }

    public override Snowflake Id => _jsonModel.Id;

    public Snowflake? TargetId => _jsonModel.TargetId;

    public IEnumerable<AuditLogChange> Changes { get; }

    public AuditLogChange<TValue> GetChange<TObject, TValue>(Expression<Func<TObject, TValue>> expression) where TObject : JsonEntity
    {
        if (_jsonModel.Changes == null)
            throw new EntityNotFoundException();

        var member = GetMemberAccess(expression);
        var name = member.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>()!.Name;

        var result = _jsonModel.Changes.FirstOrDefault(c => c.Key == name);
        if (result == null)
            throw new EntityNotFoundException();

        return new(result);
    }

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

    public AuditLogEvent ActionType => _jsonModel.ActionType.GetValueOrDefault();

    public AuditLogEntryInfo? Options { get; }

    public string? Reason => _jsonModel.Reason;

    public IEnumerable<GuildScheduledEvent> GuildScheduledEvents { get; }

    public IEnumerable<Integration> Integrations { get; }

    public IEnumerable<GuildThread> Threads { get; }

    public IEnumerable<User> Users { get; }

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

        var declaringType = memberInfo.DeclaringType;
        var parameterType = parameterExpression.Type;

        if (declaringType != null
            && declaringType != parameterType
            && declaringType.IsInterface
            && declaringType.IsAssignableFrom(parameterType)
            && memberInfo is PropertyInfo propertyInfo)
        {
            var propertyGetter = propertyInfo.GetMethod;
            var interfaceMapping = parameterType.GetTypeInfo().GetRuntimeInterfaceMap(declaringType);
            var index = Array.FindIndex(interfaceMapping.InterfaceMethods, p => p.Equals(propertyGetter));
            var targetMethod = interfaceMapping.TargetMethods[index];
            foreach (var runtimeProperty in parameterType.GetRuntimeProperties())
                if (targetMethod.Equals(runtimeProperty.GetMethod))
                    return (TMemberInfo)(object)runtimeProperty;
        }

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