using NetCord.Rest;

namespace NetCord;

public class MessageReaction : IJsonModel<JsonModels.JsonMessageReaction>
{
    JsonModels.JsonMessageReaction IJsonModel<JsonModels.JsonMessageReaction>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonMessageReaction _jsonModel;

    public int Count => _jsonModel.Count;

    public bool Me => _jsonModel.Me;

    public Snowflake? Id => _jsonModel.Emoji.Id;

    public string? Name => _jsonModel.Emoji.Name;

    public bool Animated => _jsonModel.Emoji.Animated;

    public bool IsStandard => !_jsonModel.Emoji.Id.HasValue;

    public MessageReaction(JsonModels.JsonMessageReaction jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
    }
}
