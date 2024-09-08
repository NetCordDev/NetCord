namespace NetCord;

public class Button : ICustomizableButton, IJsonModel<JsonModels.JsonComponent>
{
    JsonModels.JsonComponent IJsonModel<JsonModels.JsonComponent>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonComponent _jsonModel;

    public string CustomId => _jsonModel.CustomId!;
    public ButtonStyle Style => _jsonModel.Style.GetValueOrDefault();
    public string? Label => _jsonModel.Label;
    public EmojiReference? Emoji { get; }
    public bool Disabled => _jsonModel.Disabled.GetValueOrDefault();

    public Button(JsonModels.JsonComponent jsonModel)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = new(emoji);
    }
}
