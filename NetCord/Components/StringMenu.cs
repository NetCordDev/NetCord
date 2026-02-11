using NetCord.JsonModels;

namespace NetCord;

public class StringMenu(JsonStringMenuComponent jsonModel, int parentId) : Menu(jsonModel, parentId), IJsonModel<JsonStringMenuComponent>
{
    JsonStringMenuComponent IJsonModel<JsonStringMenuComponent>.JsonModel => GetJsonModel<JsonStringMenuComponent>();

    public IReadOnlyList<StringMenuSelectOption> Options { get; } = jsonModel.Options.SelectOrEmpty(o => new StringMenuSelectOption(o)).ToArray();

    public IReadOnlyList<string>? SelectedValues => GetJsonModel<JsonStringMenuComponent>().SelectedValues;
}
