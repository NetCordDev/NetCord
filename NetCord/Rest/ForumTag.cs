using NetCord.JsonModels;

namespace NetCord.Rest;

public class ForumTag : Entity, IJsonModel<JsonForumTag>
{
    private readonly JsonForumTag _jsonModel;

    public ForumTag(JsonForumTag jsonModel)
    {
        _jsonModel = jsonModel;
    }

    JsonForumTag IJsonModel<JsonForumTag>.JsonModel => _jsonModel;

    public override Snowflake Id => _jsonModel.Id;
    public string Name => _jsonModel.Name;
    public bool Moderated => _jsonModel.Moderated;
    public Snowflake? EmojiId => _jsonModel.EmojiId;
    public string? EmojiName => _jsonModel.EmojiName;
}
