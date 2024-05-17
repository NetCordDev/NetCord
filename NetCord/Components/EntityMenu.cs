using NetCord.JsonModels;

namespace NetCord;

public abstract class EntityMenu(JsonComponent jsonModel, IReadOnlyList<ulong> defaultValues) : Menu(jsonModel)
{
    public EntityMenu(JsonComponent jsonModel) : this(jsonModel, jsonModel.DefaultValues.SelectOrEmpty(d => d.Id).ToArray())
    {
    }

    public IReadOnlyList<ulong> DefaultValues { get; } = defaultValues;
}
