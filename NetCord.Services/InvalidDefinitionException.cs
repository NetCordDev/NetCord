using System.Reflection;

namespace NetCord.Services;

public class InvalidDefinitionException : Exception
{
    public MemberInfo Member { get; }

    public InvalidDefinitionException(string? message, MemberInfo member) : base($"{message} | {GetMemberName(member)}")
    {
        Member = member;
    }

    private static string GetMemberName(MemberInfo member)
    {
        var declaringType = member.DeclaringType;
        return declaringType is null ? member.Name : $"{declaringType.FullName}.{member.Name}";
    }
}
