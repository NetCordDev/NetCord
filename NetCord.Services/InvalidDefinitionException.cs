using System.Reflection;

namespace NetCord.Services;

public class InvalidDefinitionException(string? message, MemberInfo member) : Exception($"{message} | {GetMemberName(member)}")
{
    public MemberInfo Member { get; } = member;

    private static string GetMemberName(MemberInfo member)
    {
        var declaringType = member.DeclaringType;
        return declaringType is null ? member.Name : $"{declaringType.FullName}.{member.Name}";
    }
}
