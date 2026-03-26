using NetCord.JsonModels;

namespace NetCord;

public class LinkButton : ICustomizableButton, IJsonModel<JsonButtonComponent>
{
    JsonButtonComponent IJsonModel<JsonButtonComponent>.JsonModel => _jsonModel;
    private readonly JsonButtonComponent _jsonModel;

    public int Id => _jsonModel.Id;
    public string Url => _jsonModel.Url!;
    public string? Label => _jsonModel.Label;
    public EmojiReference? Emoji { get; }
    public bool Disabled => _jsonModel.Disabled.GetValueOrDefault();

    public LinkButton(JsonButtonComponent jsonModel)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = new(emoji);
    }
}
