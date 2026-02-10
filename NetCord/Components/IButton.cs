namespace NetCord;

public interface IButton : IActionRowComponent, IComponentSectionAccessoryComponent
{
    public bool Disabled { get; }

    public static IButton CreateFromJson(JsonModels.JsonButtonComponent jsonModel)
    {
        return jsonModel.Style switch
        {
            (ButtonStyle)5 => new LinkButton(jsonModel),
            (ButtonStyle)6 => new PremiumButton(jsonModel),
            _ => new Button(jsonModel),
        };
    }
}
