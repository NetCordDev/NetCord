using NetCord.JsonModels;

namespace NetCord;

public class ForumTag(JsonForumTag jsonModel) : Entity, IJsonModel<JsonForumTag>
{
    JsonForumTag IJsonModel<JsonForumTag>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;
    public string Name => jsonModel.Name;
    public bool Moderated => jsonModel.Moderated;
    public ulong? EmojiId => jsonModel.EmojiId;
    public string? EmojiName => jsonModel.EmojiName;
}
