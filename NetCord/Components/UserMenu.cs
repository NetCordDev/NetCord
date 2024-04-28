using NetCord.JsonModels;

namespace NetCord;

public class UserMenu(JsonComponent jsonModel) : EntityMenu(jsonModel)
{
    public IReadOnlyList<ulong> DefaultValues { get; } = jsonModel.DefaultValues.SelectOrEmpty(d => d.Id).ToArray();
}
