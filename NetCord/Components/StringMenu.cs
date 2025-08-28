using NetCord.JsonModels;

namespace NetCord;

public class StringMenu(JsonComponent jsonModel, int parentId) : Menu(jsonModel, parentId)
{
    public IReadOnlyList<StringMenuSelectOption> Options { get; } = jsonModel.Options.SelectOrEmpty(o => new StringMenuSelectOption(o)).ToArray();

    public IReadOnlyList<string>? SelectedValues => _jsonModel.SelectedValues;
}
