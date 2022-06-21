namespace NetCord.Rest;

public class ReactionEmojiProperties
{
    public string Name { get; }
    public Snowflake? Id { get; }
    public Type EmojiType { get; }

    /// <summary>
    /// Creates <see cref="ReactionEmojiProperties"/> from guild emoji
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    public ReactionEmojiProperties(string name, Snowflake id)
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