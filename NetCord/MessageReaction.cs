namespace NetCord;

public class MessageReaction : IJsonModel<JsonModels.JsonMessageReaction>
{
    JsonModels.JsonMessageReaction IJsonModel<JsonModels.JsonMessageReaction>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonMessageReaction _jsonModel;

    public int Count => _jsonModel.Count;

    public MessageReactionCountDetails CountDetails { get; }

    public bool Me => _jsonModel.Me;

    public bool MeBurst => _jsonModel.MeBurst;

    public ulong? Id => _jsonModel.Emoji.Id;

    public string? Name => _jsonModel.Emoji.Name;

    public bool Animated => _jsonModel.Emoji.Animated;

    public bool IsStandard => !_jsonModel.Emoji.Id.HasValue;

    public IReadOnlyList<Color> BurstColors => _jsonModel.BurstColors;

    public MessageReaction(JsonModels.JsonMessageReaction jsonModel)
    {
        _jsonModel = jsonModel;
        CountDetails = new(jsonModel.CountDetails);
    }
}
