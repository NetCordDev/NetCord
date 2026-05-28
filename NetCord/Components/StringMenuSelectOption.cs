namespace NetCord;

public class StringMenuSelectOption : IJsonModel<JsonModels.JsonStringMenuSelectOption>
{
    JsonModels.JsonStringMenuSelectOption IJsonModel<JsonModels.JsonStringMenuSelectOption>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonStringMenuSelectOption _jsonModel;

    public string Label => _jsonModel.Label;
    public string Value => _jsonModel.Value;
    public string? Description => _jsonModel.Description;
    public EmojiReference? Emoji { get; }
    public bool Default => _jsonModel.Default;

    public StringMenuSelectOption(JsonModels.JsonStringMenuSelectOption jsonModel)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = new(emoji);
    }
}
