using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class SlashCommandAttribute : ApplicationCommandAttribute
{
    public SlashCommandAttribute(string name, string description) : base(name)
    {
        Description = description;
    }

    public string Description { get; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? DescriptionTranslationsProviderType { get; init; }
}
