using NetCord.JsonModels;

namespace NetCord;

public class MentionableMenuDefaultValue(JsonEntitySelectDefaultValue jsonModel) : Entity(jsonModel)
{
    public MentionableMenuDefaultValueType Type { get; } = (MentionableMenuDefaultValueType)jsonModel.Type;
}
