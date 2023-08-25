using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SubSlashCommandAttribute : Attribute
{
    public SubSlashCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? NameTranslationsProviderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }

    public string Description { get; }

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? DescriptionTranslationsProviderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }
}
