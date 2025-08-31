namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial interface IButtonProperties : IActionRowComponentProperties, IComponentSectionAccessoryComponentProperties
{
    /// <summary>
    /// Style of the button.
    /// </summary>
    public ButtonStyle Style { get; }

    /// <summary>
    /// Whether the button is disabled.
    /// </summary>
    public bool Disabled { get; set; }
}
