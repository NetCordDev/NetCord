namespace NetCord;

public interface IButton : IComponentSectionAccessory
{
    public int Id { get; }
    public bool Disabled { get; }

    public new static IButton CreateFromJson(JsonModels.JsonComponent jsonModel)
    {
        return jsonModel.Style.GetValueOrDefault() switch
        {
            (ButtonStyle)5 => new LinkButton(jsonModel),
            (ButtonStyle)6 => new PremiumButton(jsonModel),
            _ => new Button(jsonModel),
        };
    }
}
