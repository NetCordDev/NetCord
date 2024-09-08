namespace NetCord;

public interface ICustomizableButton : IButton
{
    public string? Label { get; }
    public EmojiReference? Emoji { get; }
}
