namespace NetCord.Rest;

public partial interface ICustomizableButtonProperties : IButtonProperties
{
    /// <summary>
    /// Text that appears on the button (max 80 characters).
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Emoji that appears on the button.
    /// </summary>
    public EmojiProperties? Emoji { get; set; }
}
