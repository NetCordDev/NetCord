using NetCord.JsonModels;

namespace NetCord;

public class RoleMenu : EntityMenu, IJsonModel<JsonRoleMenuComponent>
{
    JsonRoleMenuComponent IJsonModel<JsonRoleMenuComponent>.JsonModel => GetJsonModel<JsonRoleMenuComponent>();

    public RoleMenu(JsonRoleMenuComponent jsonModel, int parentId) : base(jsonModel, parentId)
    {
    }

    public unsafe RoleMenu(JsonRoleMenuComponent jsonModel,
                           int parentId,
                           InteractionResolvedData? resolvedData) : base(jsonModel,
                                                                         parentId,
                                                                         GetSelectedValues(jsonModel, &EntityMenuHelper.GetRoleValues, out var selectedValues, resolvedData))
    {
        SelectedValues = selectedValues;
    }

    public new IReadOnlyList<Role>? SelectedValues { get; }
}
