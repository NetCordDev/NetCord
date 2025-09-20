using NetCord.JsonModels;

namespace NetCord;

public class RoleMenu : EntityMenu
{
    public RoleMenu(JsonComponent jsonModel, int parentId) : base(jsonModel, parentId)
    {
    }

    public unsafe RoleMenu(JsonComponent jsonModel,
                           int parentId,
                           InteractionResolvedData? resolvedData) : base(jsonModel,
                                                                         parentId,
                                                                         GetSelectedValues(jsonModel, &EntityMenuHelper.GetRoleValues, out var selectedValues, resolvedData))
    {
        SelectedValues = selectedValues;
    }

    public new IReadOnlyList<Role>? SelectedValues { get; }
}
