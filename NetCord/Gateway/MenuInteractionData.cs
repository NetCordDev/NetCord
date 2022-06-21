using System.Collections.ObjectModel;

namespace NetCord.Gateway;

public class MenuInteractionData : ButtonInteractionData, ICustomIdInteractionData
{
    public ReadOnlyCollection<string> SelectedValues { get; }

    public MenuInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
        SelectedValues = new(jsonModel.SelectedValues!);
    }
}