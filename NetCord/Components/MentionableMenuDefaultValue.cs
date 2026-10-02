using NetCord.JsonModels;

namespace NetCord;

public class MentionableMenuDefaultValue(JsonEntitySelectDefaultValue jsonModel) : Entity
{
    public override ulong Id { get; } = jsonModel.Id;

    public MentionableMenuDefaultValueType Type { get; } = (MentionableMenuDefaultValueType)jsonModel.Type;
}
