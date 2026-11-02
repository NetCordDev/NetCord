using NetCord.JsonModels;

namespace NetCord;

public class MentionableSelectDefaultValue(JsonEntitySelectDefaultValue jsonModel) : Entity(jsonModel)
{
    public MentionableSelectDefaultValueType Type { get; } = (MentionableSelectDefaultValueType)jsonModel.Type;
}
