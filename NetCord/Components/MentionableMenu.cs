using NetCord.JsonModels;

namespace NetCord;

public class MentionableMenu(JsonComponent jsonModel) : EntityMenu(jsonModel)
{
    public IReadOnlyList<MentionableMenuDefaultValue> DefaultValues { get; } = jsonModel.DefaultValues!.Select(d => new MentionableMenuDefaultValue(d)).ToArray();
}
