namespace NetCord;

public class ReactionEmoji
{
    public string Name { get; }
    public DiscordId? Id { get; }
    public Type EmojiType { get; }

    /// <summary>
    /// Creates <see cref="ReactionEmoji"/> from guild emoji
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    public ReactionEmoji(string name, DiscordId? id)
    {
        Name = name;
        Id = id;
        EmojiType = Type.Guild;
    }

    /// <summary>
    /// Creates <see cref="ReactionEmoji"/> from standard Discord emoji
    /// </summary>
    /// <param name="unicode"></param>
    public ReactionEmoji(string unicode)
    {
        Name = unicode;
        EmojiType = Type.Standard;
    }

    public enum Type
    {
        Guild,
        Standard,
    }
}