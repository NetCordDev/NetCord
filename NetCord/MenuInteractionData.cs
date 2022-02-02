namespace NetCord;

public class MenuInteractionData : ButtonInteractionData
{
    public IReadOnlyCollection<string> SelectedValues { get; }

    internal MenuInteractionData(JsonModels.JsonInteractionData jsonEntity) : base(jsonEntity)
    {
        SelectedValues = jsonEntity.SelectedValues!;
    }
}