namespace NetCord;

public class MessageReactionEmoji : IJsonModel<JsonModels.JsonEmoji>
{
    JsonModels.JsonEmoji IJsonModel<JsonModels.JsonEmoji>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmoji _jsonModel;

    public MessageReactionEmoji(JsonModels.JsonEmoji jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong? Id => _jsonModel.Id;

    public string? Name => _jsonModel.Name;

    public bool Animated => _jsonModel.Animated;
}
