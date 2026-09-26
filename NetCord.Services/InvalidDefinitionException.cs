using System.Reflection;

namespace NetCord.Services;

public class InvalidDefinitionException(string message, string name) : Exception($"{message} | {name}")
{
    public InvalidDefinitionException(string message, MemberInfo member) : this(message, GetMemberName(member))
    {
        Member = member;
    }

    public string Name { get; } = name;

    public MemberInfo? Member { get; }

    private static string GetMemberName(MemberInfo member)
    {
        var declaringType = member.DeclaringType;
        return declaringType is null ? member.Name : $"{declaringType.FullName}.{member.Name}";
    }
}
