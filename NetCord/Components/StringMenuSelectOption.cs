namespace NetCord;

public class StringMenuSelectOption : IJsonModel<JsonModels.JsonMenuSelectOption>
{
    JsonModels.JsonMenuSelectOption IJsonModel<JsonModels.JsonMenuSelectOption>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonMenuSelectOption _jsonModel;

    public string Label => _jsonModel.Label;
    public string Value => _jsonModel.Value;
    public string? Description => _jsonModel.Description;
    public ComponentEmoji? Emoji { get; }
    public bool Default => _jsonModel.Default;

    public StringMenuSelectOption(JsonModels.JsonMenuSelectOption jsonModel)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = new(emoji);
    }
}
