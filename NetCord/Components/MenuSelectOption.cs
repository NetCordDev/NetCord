using NetCord.JsonModels;

namespace NetCord;

public class MenuSelectOption
{
    private readonly JsonMenuSelectOption _jsonModel;

    public string Label => _jsonModel.Label;
    public string Value => _jsonModel.Value;
    public string? Description => _jsonModel.Description;
    public ComponentEmoji? Emoji { get; }
    public bool Default => _jsonModel.Default.GetValueOrDefault();

    public MenuSelectOption(JsonMenuSelectOption jsonModel)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Emoji != null)
            Emoji = new(jsonModel.Emoji);
    }
}