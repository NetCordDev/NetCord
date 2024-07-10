using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

using NetCord.JsonModels;

namespace NetCord.Gateway;

public class AuditLogEntry : Entity, IJsonModel<JsonAuditLogEntry>
{
    JsonAuditLogEntry IJsonModel<JsonAuditLogEntry>.JsonModel => _jsonModel;
    private protected readonly JsonAuditLogEntry _jsonModel;

    public AuditLogEntry(JsonAuditLogEntry jsonModel)
    {
        _jsonModel = jsonModel;
        Changes = _jsonModel.Changes.ToDictionaryOrEmpty(c => c.Key, c => new AuditLogChange(c));

        var options = _jsonModel.Options;
        if (options is not null)
            Options = new(options);
    }

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// ID of the affected entity.
    /// </summary>
    public ulong? TargetId => _jsonModel.TargetId;

    /// <summary>
    /// Changes made to the <see cref="TargetId"/>.
    /// </summary>
    public IReadOnlyDictionary<string, AuditLogChange> Changes { get; }

    /// <summary>
    /// ID of user that made the changes.
    /// </summary>
    public ulong? UserId => _jsonModel.UserId;

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

    private bool TryGetChangeModel<TObject, TValue>(Expression<Func<TObject, TValue?>> expression, [NotNullWhen(true)] out JsonAuditLogChange model)
    {
        var member = GetMemberAccess(expression);
        var name = member.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>()!.Name;

        if (Changes.TryGetValue(name, out var result))
        {
            model = ((IJsonModel<JsonAuditLogChange>)result).JsonModel;
            return true;
        }

        model = null!;
        return false;
    }

    /// <summary>
    /// Tries to find specified change based on <paramref name="expression"/>.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="expression">Expression finding the change, for example: <c>channel => channel.Name</c>.</param>
    /// <param name="change">The result.</param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    public bool TryGetChange<TObject, TValue>(Expression<Func<TObject, TValue?>> expression, [NotNullWhen(true)] out AuditLogChange<TValue> change) where TObject : JsonEntity
    {
        if (TryGetChangeModel(expression, out var model))
        {
            change = new(model);
            return true;
        }

        change = null!;
        return false;
    }

    /// <summary>
    /// Finds specified change based on <paramref name="expression"/>.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="expression">Expression finding the change, for example: <c>channel => channel.Name</c>.</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    public AuditLogChange<TValue> GetChange<TObject, TValue>(Expression<Func<TObject, TValue?>> expression) where TObject : JsonEntity
    {
        if (TryGetChange(expression, out var value))
            return value;

        throw new EntityNotFoundException();
    }

    /// <summary>
    /// Tries to find specified change based on <paramref name="expression"/>.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="expression">Expression finding the change, for example: <c>channel => channel.Name</c>.</param>
    /// <param name="jsonTypeInfo"><see cref="JsonTypeInfo{TValue}"/> of the object returned by <paramref name="expression"/>.</param>
    /// <param name="change">The result.</param>
    /// <returns></returns>
    public bool TryGetChange<TObject, TValue>(Expression<Func<TObject, TValue?>> expression, JsonTypeInfo<TValue> jsonTypeInfo, [NotNullWhen(true)] out AuditLogChange<TValue> change) where TObject : JsonEntity
    {
        if (TryGetChangeModel(expression, out var model))
        {
            change = new(model, jsonTypeInfo);
            return true;
        }

        change = null!;
        return false;
    }

    /// <summary>
    /// Finds specified change based on <paramref name="expression"/>.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="expression">Expression finding the change, for example: <c>channel => channel.Name</c>.</param>
    /// <param name="jsonTypeInfo"><see cref="JsonTypeInfo{TValue}"/> of the object returned by <paramref name="expression"/>.</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public AuditLogChange<TValue> GetChange<TObject, TValue>(Expression<Func<TObject, TValue?>> expression, JsonTypeInfo<TValue> jsonTypeInfo) where TObject : JsonEntity
    {
        if (TryGetChange(expression, jsonTypeInfo, out var value))
            return value;

        throw new EntityNotFoundException();
    }

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

        return memberInfo is null
            ? throw new ArgumentException($"The expression '{nameof(memberAccessExpression)}' is not a valid member access expression. The expression should represent a simple property or field access: 't => t.MyProperty'.", nameof(memberAccessExpression))
            : memberInfo;
    }

    private static TMemberInfo? MatchSimpleMemberAccess<TMemberInfo>(
    Expression parameterExpression,
    Expression memberAccessExpression)
    where TMemberInfo : MemberInfo
    {
        var memberInfos = MatchMemberAccess<TMemberInfo>(parameterExpression, memberAccessExpression);

        return memberInfos?.Count == 1 ? memberInfos[0] : null;
    }

    private static List<TMemberInfo>? MatchMemberAccess<TMemberInfo>(
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
