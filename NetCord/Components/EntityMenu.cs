using NetCord.JsonModels;

namespace NetCord;

public abstract class EntityMenu : Menu, IJsonModel<JsonEntityMenuComponent>
{
    JsonEntityMenuComponent IJsonModel<JsonEntityMenuComponent>.JsonModel => GetJsonModel<JsonEntityMenuComponent>();

    private protected EntityMenu(JsonEntityMenuComponent jsonModel, int parentId) : base(jsonModel, parentId)
    {
        DefaultValues = jsonModel.DefaultValues.SelectOrEmpty(v => v.Id).ToArray();
    }

    private protected EntityMenu(JsonEntityMenuComponent jsonModel, IReadOnlyList<ulong> defaultValues, int parentId) : base(jsonModel, parentId)
    {
        DefaultValues = defaultValues;
    }

    private protected EntityMenu(JsonEntityMenuComponent jsonModel, int parentId, IReadOnlyList<ulong> selectedValues) : base(jsonModel, parentId)
    {
        DefaultValues = jsonModel.DefaultValues.SelectOrEmpty(v => v.Id).ToArray();

        SelectedValues = selectedValues;
    }

    private protected EntityMenu(JsonEntityMenuComponent jsonModel, IReadOnlyList<ulong> defaultValues, int parentId, IReadOnlyList<ulong> selectedValues) : base(jsonModel, parentId)
    {
        DefaultValues = defaultValues;

        SelectedValues = selectedValues;
    }

    private protected static unsafe IReadOnlyList<ulong> GetSelectedValues<T>(JsonEntityMenuComponent jsonModel, delegate*<IEnumerable<ulong>, InteractionResolvedData, T[]> getValues, out T[] values, InteractionResolvedData? resolvedData) where T : Entity
    {
        if (resolvedData is not null)
            return new EntityArrayWrapper<T>(values = getValues(jsonModel.SelectedValues!, resolvedData));

        values = [];
        return [];
    }

    public IReadOnlyList<ulong> DefaultValues { get; }

    public IReadOnlyList<ulong>? SelectedValues { get; }
}
