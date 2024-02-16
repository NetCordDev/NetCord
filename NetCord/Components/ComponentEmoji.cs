namespace NetCord;

public class ComponentEmoji(JsonModels.JsonEmoji jsonModel) : Entity, IJsonModel<JsonModels.JsonEmoji>
{
    JsonModels.JsonEmoji IJsonModel<JsonModels.JsonEmoji>.JsonModel => jsonModel;

    public override ulong Id
    {
        get
        {
            if (IsStandard)
                throw new InvalidOperationException("This emoji has no id.");
            return jsonModel.Id.GetValueOrDefault();
        }
    }

    public bool IsStandard => !jsonModel.Id.HasValue;

    public string Name => jsonModel.Name!;

    public bool Animated => jsonModel.Animated;
}
