namespace NetCord;

public class ReactionEmojiProperties
{
    public string Name { get; }
    public DiscordId? Id { get; }
    public Type EmojiType { get; }

    /// <summary>
    /// Creates <see cref="ReactionEmojiProperties"/> from guild emoji
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    public ReactionEmojiProperties(string name, DiscordId? id)
    {
        Name = name;
        Id = id;
        EmojiType = Type.Guild;
    }

    /// <summary>
    /// Creates <see cref="ReactionEmojiProperties"/> from standard Discord emoji
    /// </summary>
    /// <param name="unicode"></param>
    public ReactionEmojiProperties(string unicode)
    {
        Name = unicode;
        EmojiType = Type.Standard;
    }

    public enum Type
    {
        Guild,
        Standard,
    }

    public static implicit operator ReactionEmojiProperties(string unicode) => new(unicode);
}