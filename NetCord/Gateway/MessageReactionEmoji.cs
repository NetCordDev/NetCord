namespace NetCord.Gateway;

public class MessageReactionEmoji : Entity, IJsonModel<JsonModels.JsonEmoji>
{
    JsonModels.JsonEmoji IJsonModel<JsonModels.JsonEmoji>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmoji _jsonModel;

    public MessageReactionEmoji(JsonModels.JsonEmoji jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public override Snowflake Id
    {
        get
        {
            if (IsStandard)
                throw new InvalidOperationException("This emoji has no id");
            return _jsonModel.Id.GetValueOrDefault();
        }
    }

    public bool IsStandard => !_jsonModel.Id.HasValue;

    public string? Name => _jsonModel.Name;

    public bool Animated => _jsonModel.Animated;
}
