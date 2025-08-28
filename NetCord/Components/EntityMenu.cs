using NetCord.JsonModels;

namespace NetCord;

public abstract class EntityMenu : Menu
{
    public EntityMenu(JsonComponent jsonModel, IReadOnlyList<ulong> defaultValues, int parentId) : base(jsonModel, parentId)
    {
        DefaultValues = defaultValues;

        if (jsonModel.SelectedValues is { Length: int length } selectedValues)
        {
            var result = new ulong[length];
            for (var i = 0; i < length; i++)
                result[i] = Snowflake.Parse(selectedValues[i]);
            SelectedValues = result;
        }
    }

    public EntityMenu(JsonComponent jsonModel, int parentId) : this(jsonModel, jsonModel.DefaultValues.SelectOrEmpty(d => d.Id).ToArray(), parentId)
    {
    }

    public IReadOnlyList<ulong> DefaultValues { get; }

    public IReadOnlyList<ulong>? SelectedValues { get; }
}
