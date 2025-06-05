using NetCord.JsonModels;

namespace NetCord;

public abstract class EntityMenu(JsonComponent jsonModel, IReadOnlyList<ulong> defaultValues, int parentId) : Menu(jsonModel, parentId)
{
    public EntityMenu(JsonComponent jsonModel, int parentId) : this(jsonModel, jsonModel.DefaultValues.SelectOrEmpty(d => d.Id).ToArray(), parentId)
    {
    }

    public IReadOnlyList<ulong> DefaultValues { get; } = defaultValues;
}
