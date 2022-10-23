using NetCord.JsonModels;

namespace NetCord;

public class ForumGuildChannelDefaultReaction : IJsonModel<JsonForumGuildChannelDefaultReaction>
{
    private readonly JsonForumGuildChannelDefaultReaction _jsonModel;
    JsonForumGuildChannelDefaultReaction IJsonModel<JsonForumGuildChannelDefaultReaction>.JsonModel => _jsonModel;

    public ForumGuildChannelDefaultReaction(JsonForumGuildChannelDefaultReaction jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong? EmojiId => _jsonModel.EmojiId;
    public string? EmojiName => _jsonModel.EmojiName;
}
