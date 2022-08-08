namespace NetCord;

public class ComponentEmoji : Entity, IJsonModel<JsonModels.JsonEmoji>
{
    JsonModels.JsonEmoji IJsonModel<JsonModels.JsonEmoji>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmoji _jsonModel;

    public override Snowflake Id
    {
        get
        {
            if (IsStandard)
                throw new InvalidOperationException("This emoji has no id.");
            return _jsonModel.Id.GetValueOrDefault();
        }
    }

    public bool IsStandard => !_jsonModel.Id.HasValue;

    public string Name => _jsonModel.Name!;

    public bool Animated => _jsonModel.Animated;

    public ComponentEmoji(JsonModels.JsonEmoji jsonModel)
    {
        _jsonModel = jsonModel;
    }
}