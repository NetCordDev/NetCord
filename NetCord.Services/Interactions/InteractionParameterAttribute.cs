using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.Interactions;

[AttributeUsage(AttributeTargets.Parameter)]
public class InteractionParameterAttribute : Attribute
{
    public Type? TypeReaderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }
}
