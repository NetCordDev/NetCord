using System.Collections.ObjectModel;

namespace NetCord;

public class MenuInteractionData : ButtonInteractionData, ICustomIdInteractionData
{
    public ReadOnlyCollection<string> SelectedValues { get; }

    internal MenuInteractionData(JsonModels.JsonInteractionData jsonEntity) : base(jsonEntity)
    {
        SelectedValues = new(jsonEntity.SelectedValues!);
    }
}