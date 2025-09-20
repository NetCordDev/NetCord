using NetCord.JsonModels;

namespace NetCord;

public class UserMenu : EntityMenu
{
    public UserMenu(JsonComponent jsonModel, int parentId) : base(jsonModel, parentId)
    {
    }

    public unsafe UserMenu(JsonComponent jsonModel,
                           int parentId,
                           InteractionResolvedData? resolvedData) : base(jsonModel,
                                                                         parentId,
                                                                         GetSelectedValues(jsonModel, &EntityMenuHelper.GetUserValues, out var selectedValues, resolvedData))
    {
        SelectedValues = selectedValues;
    }

    public new IReadOnlyList<User>? SelectedValues { get; }
}
