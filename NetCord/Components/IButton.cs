namespace NetCord;

public interface IButton
{
    public string? Label { get; }
    public ComponentEmoji? Emoji { get; }
    public bool Disabled { get; }

    public static IButton CreateFromJson(JsonModels.JsonComponent jsonModel)
    {
        return jsonModel.Style.GetValueOrDefault() switch
        {
            (ButtonStyle)5 => new LinkButton(jsonModel),
            _ => new Button(jsonModel),
        };
    }
}
