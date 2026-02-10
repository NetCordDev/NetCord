using NetCord.JsonModels;

namespace NetCord;

public class Button : IInteractiveComponent, ICustomizableButton, IJsonModel<JsonButtonComponent>
{
    JsonButtonComponent IJsonModel<JsonButtonComponent>.JsonModel => _jsonModel;
    private readonly JsonButtonComponent _jsonModel;

    public int Id => _jsonModel.Id;
    public string CustomId => _jsonModel.CustomId;
    public ButtonStyle Style => _jsonModel.Style;
    public string? Label => _jsonModel.Label;
    public EmojiReference? Emoji { get; }
    public bool Disabled => _jsonModel.Disabled.GetValueOrDefault();

    public Button(JsonButtonComponent jsonModel)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = new(emoji);
    }
}
