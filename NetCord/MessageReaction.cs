namespace NetCord;

public class MessageReaction : IJsonModel<JsonModels.JsonMessageReaction>
{
    JsonModels.JsonMessageReaction IJsonModel<JsonModels.JsonMessageReaction>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonMessageReaction _jsonModel;

    public int Count => _jsonModel.Count;

    public MessageReactionCountDetails CountDetails { get; }

    public bool Me => _jsonModel.Me;

    public bool MeBurst => _jsonModel.MeBurst;

    public MessageReactionEmoji Emoji { get; }

    public IReadOnlyList<Color> BurstColors => _jsonModel.BurstColors;

    public MessageReaction(JsonModels.JsonMessageReaction jsonModel)
    {
        _jsonModel = jsonModel;
        CountDetails = new(jsonModel.CountDetails);
        Emoji = new(jsonModel.Emoji);
    }
}
