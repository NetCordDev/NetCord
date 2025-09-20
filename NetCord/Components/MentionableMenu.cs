using NetCord.JsonModels;

namespace NetCord;

public class MentionableMenu : EntityMenu
{
    public MentionableMenu(JsonComponent jsonModel, int parentId) : base(jsonModel,
                                                                         GetDefaultValues(jsonModel, out var defaultValues),
                                                                         parentId)
    {
        DefaultValues = defaultValues;
    }

    public unsafe MentionableMenu(JsonComponent jsonModel,
                                  int parentId,
                                  InteractionResolvedData? resolvedData) : base(jsonModel,
                                                                                GetDefaultValues(jsonModel, out var defaultValues),
                                                                                parentId,
                                                                                GetSelectedValues(jsonModel, &EntityMenuHelper.GetMentionableValues, out var selectedValues, resolvedData))
    {
        DefaultValues = defaultValues;

        SelectedValues = selectedValues;
    }

    private static EntityArrayWrapper<MentionableMenuDefaultValue> GetDefaultValues(JsonComponent jsonModel,
                                                                                    out MentionableMenuDefaultValue[] defaultValues)
        => new(defaultValues = jsonModel.DefaultValues.SelectOrEmpty(d => new MentionableMenuDefaultValue(d)).ToArray());

    public new IReadOnlyList<MentionableMenuDefaultValue> DefaultValues { get; }

    public new IReadOnlyList<Mentionable>? SelectedValues { get; }
}
