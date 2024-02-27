using NetCord.JsonModels;

namespace NetCord;

public class StringMenu(JsonComponent jsonModel) : Menu(jsonModel)
{
    public IReadOnlyList<StringMenuSelectOption> Options { get; } = jsonModel.Options.Select(o => new StringMenuSelectOption(o)).ToArray();
}
