using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

namespace NetCord.Services;

[Serializable]
public class InvalidDefinitionException : Exception
{
    [AllowNull]
    [field: NonSerialized]
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

    protected InvalidDefinitionException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
