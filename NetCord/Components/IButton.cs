namespace NetCord;

public interface IButton
{
    public bool Disabled { get; }

    public static IButton CreateFromJson(JsonModels.JsonComponent jsonModel)
    {
        return jsonModel.Style.GetValueOrDefault() switch
        {
            (ButtonStyle)5 => new LinkButton(jsonModel),
            (ButtonStyle)6 => new PremiumButton(jsonModel),
            _ => new Button(jsonModel),
        };
    }
}
