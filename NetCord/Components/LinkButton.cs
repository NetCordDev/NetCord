using NetCord.JsonModels;

namespace NetCord;

public class LinkButton : IButton, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private readonly JsonComponent _jsonModel;

    public string Url => _jsonModel.Url!;
    public string? Label => _jsonModel.Label;
    public ComponentEmoji? Emoji { get; }
    public bool Disabled => _jsonModel.Disabled.GetValueOrDefault();

    public LinkButton(JsonComponent jsonModel)
    {
        _jsonModel = jsonModel;

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = new(emoji);
    }
}
