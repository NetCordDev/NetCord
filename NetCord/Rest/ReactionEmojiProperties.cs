namespace NetCord.Rest;

public partial class ReactionEmojiProperties
{
    public string Name { get; set; }
    public ulong? Id { get; set; }

    /// <summary>
    /// Creates <see cref="ReactionEmojiProperties"/> from guild emoji
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    public ReactionEmojiProperties(string name, ulong id)
    {
        Name = name;
        Id = id;
    }

    /// <summary>
    /// Creates <see cref="ReactionEmojiProperties"/> from standard Discord emoji
    /// </summary>
    /// <param name="unicode"></param>
    public ReactionEmojiProperties(string unicode)
    {
        Name = unicode;
    }

    public static implicit operator ReactionEmojiProperties(string unicode) => new(unicode);
}
