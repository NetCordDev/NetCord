using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a tag that can be applied to a <see cref="ForumGuildThread"/>.
/// </summary>
public class ForumTag(JsonForumTag jsonModel) : Entity
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    public override ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// The name of the tag, between 0 and 20 characters.
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// Whether this tag can only be added/removed using the <see cref="Permissions.ManageThreads"/> permission.
    /// </summary>
    public bool Moderated { get; } = jsonModel.Moderated;

    /// <summary>
    /// The ID of the custom emoji to use for the tag.
    /// </summary>
    /// <remarks>
    /// Cannot be set alongside <see cref="EmojiName"/>.
    /// </remarks>
    public ulong? EmojiId { get; } = jsonModel.EmojiId;

    /// <summary>
    /// The unicode emoji to use for the tag.
    /// </summary>
    /// <remarks>
    /// Cannot be set alongside <see cref="EmojiId"/>.
    /// </remarks>
    public string? EmojiName { get; } = jsonModel.EmojiName;
}
