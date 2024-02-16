using NetCord.JsonModels;

namespace NetCord;

public class ForumGuildChannelDefaultReaction(JsonForumGuildChannelDefaultReaction jsonModel) : IJsonModel<JsonForumGuildChannelDefaultReaction>
{
    JsonForumGuildChannelDefaultReaction IJsonModel<JsonForumGuildChannelDefaultReaction>.JsonModel => jsonModel;

    public ulong? EmojiId => jsonModel.EmojiId;
    public string? EmojiName => jsonModel.EmojiName;
}
