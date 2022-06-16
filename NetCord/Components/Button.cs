namespace NetCord;

public abstract class Button : IJsonModel<JsonModels.JsonComponent>
{
    JsonModels.JsonComponent IJsonModel<JsonModels.JsonComponent>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonComponent _jsonModel;

    public ComponentType ComponentType => ComponentType.Button;

    public string? Label => _jsonModel.Label;

    public ComponentEmoji? Emoji { get; }

    public bool Disabled => _jsonModel.Disabled;

    private protected Button(JsonModels.JsonComponent jsonModel)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Emoji != null)
            Emoji = new(jsonModel.Emoji);
    }

    public static Button CreateFromJson(JsonModels.JsonComponent jsonModel)
    {
        return (int)jsonModel.Style! switch
        {
            5 => new LinkButton(jsonModel),
            _ => new ActionButton(jsonModel),
        };
    }
}