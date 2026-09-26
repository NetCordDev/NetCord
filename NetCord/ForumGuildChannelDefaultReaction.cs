using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents the default displayed reaction for threads within a <see cref="ForumGuildChannel"/> or <see cref="MediaForumGuildChannel"/>.
/// </summary>
public class ForumGuildChannelDefaultReaction(JsonForumGuildChannelDefaultReaction jsonModel) : IJsonModel<JsonForumGuildChannelDefaultReaction>
{
    JsonForumGuildChannelDefaultReaction IJsonModel<JsonForumGuildChannelDefaultReaction>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the custom guild emoji to use for the reaction.
    /// </summary>
    /// <remarks>
    /// Cannot be set alongside <see cref="EmojiName"/>.
    /// </remarks>
    public ulong? EmojiId => jsonModel.EmojiId;

    /// <summary>
    /// The unicode emoji to use for the reaction.
    /// </summary>
    /// <remarks>
    /// Cannot be set alongside <see cref="EmojiId"/>.
    /// </remarks>
    public string? EmojiName => jsonModel.EmojiName;
}
