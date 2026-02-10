using NetCord.JsonModels;

namespace NetCord;

public class ChannelMenu : EntityMenu, IJsonModel<JsonChannelMenuComponent>
{
    JsonChannelMenuComponent IJsonModel<JsonChannelMenuComponent>.JsonModel => GetJsonModel<JsonChannelMenuComponent>();

    public ChannelMenu(JsonChannelMenuComponent jsonModel, int parentId) : base(jsonModel, parentId)
    {
        ChannelTypes = jsonModel.ChannelTypes ?? [];
    }

    public unsafe ChannelMenu(JsonChannelMenuComponent jsonModel,
                              int parentId,
                              InteractionResolvedData? resolvedData) : base(jsonModel,
                                                                            parentId,
                                                                            GetSelectedValues(jsonModel, &EntityMenuHelper.GetChannelValues, out var selectedValues, resolvedData))
    {
        ChannelTypes = jsonModel.ChannelTypes ?? [];

        SelectedValues = selectedValues;
    }

    public IReadOnlyList<ChannelType> ChannelTypes { get; }

    public new IReadOnlyList<Channel>? SelectedValues { get; }
}
