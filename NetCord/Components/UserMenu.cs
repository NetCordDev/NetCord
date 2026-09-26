using NetCord.JsonModels;

namespace NetCord;

public class UserMenu : EntityMenu, IJsonModel<JsonUserMenuComponent>
{
    JsonUserMenuComponent IJsonModel<JsonUserMenuComponent>.JsonModel => GetJsonModel<JsonUserMenuComponent>();

    public UserMenu(JsonUserMenuComponent jsonModel, int parentId) : base(jsonModel, parentId)
    {
    }

    public unsafe UserMenu(JsonUserMenuComponent jsonModel,
                           int parentId,
                           InteractionResolvedData? resolvedData) : base(jsonModel,
                                                                         parentId,
                                                                         GetSelectedValues(jsonModel, &EntityMenuHelper.GetUserValues, out var selectedValues, resolvedData))
    {
        SelectedValues = selectedValues;
    }

    public new IReadOnlyList<User>? SelectedValues { get; }
}
