using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Field)]
public class SlashCommandChoiceAttribute : Attribute
{
    public string? Name { get; init; }

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TranslationsProviderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }
}
