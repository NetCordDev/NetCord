namespace NetCord;

public class StringSelectOption : IJsonModel<JsonModels.JsonStringSelectOption>
{
    JsonModels.JsonStringSelectOption IJsonModel<JsonModels.JsonStringSelectOption>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonStringSelectOption _jsonModel;

    public string Label => _jsonModel.Label;
    public string Value => _jsonModel.Value;
    public string? Description => _jsonModel.Description;
    public EmojiReference? Emoji { get; }
    public bool Default => _jsonModel.Default;

    public StringSelectOption(JsonModels.JsonStringSelectOption jsonModel)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = new(emoji);
    }
}
