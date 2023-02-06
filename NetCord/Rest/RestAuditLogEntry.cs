using System.Linq.Expressions;
using System.Reflection;

using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord.Rest;

public class RestAuditLogEntry : AuditLogEntry, IJsonModel<JsonAuditLogEntry>
{
    public RestAuditLogEntry(JsonAuditLogEntry jsonModel, RestAuditLogEntryData data) : base(jsonModel)
    {
        Data = data;
    }

    /// <summary>
    /// Data of objects referenced in the audit log.
    /// </summary>
    public RestAuditLogEntryData Data { get; }

    /// <summary>
    /// User that made the changes.
    /// </summary>
    public User? User
    {
        get
        {
            var userId = UserId;
            return userId.HasValue ? Data.Users[userId.GetValueOrDefault()] : null;
        }
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
