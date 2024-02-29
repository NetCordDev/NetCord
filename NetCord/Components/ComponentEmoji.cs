namespace NetCord;

public class ComponentEmoji(JsonModels.JsonEmoji jsonModel) : IJsonModel<JsonModels.JsonEmoji>
{
    JsonModels.JsonEmoji IJsonModel<JsonModels.JsonEmoji>.JsonModel => jsonModel;

    public ulong? Id => jsonModel.Id;

    public string Name => jsonModel.Name!;

    public bool Animated => jsonModel.Animated;
}
