using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SlashCommandAttribute : ApplicationCommandAttribute
{
    public SlashCommandAttribute(string name, string description) : base(name)
    {
        Description = description;
    }

    public string Description { get; }

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? DescriptionTranslationsProviderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }
}
