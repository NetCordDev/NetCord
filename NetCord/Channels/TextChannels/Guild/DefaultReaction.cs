using NetCord.JsonModels;

namespace NetCord;

public class DefaultReaction : IJsonModel<JsonDefaultReaction>
{
    private readonly JsonDefaultReaction _jsonModel;
    JsonDefaultReaction IJsonModel<JsonDefaultReaction>.JsonModel => _jsonModel;

    public DefaultReaction(JsonDefaultReaction jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake? EmojiId => _jsonModel.EmojiId;
    public string? EmojiName => _jsonModel.EmojiName;
}
